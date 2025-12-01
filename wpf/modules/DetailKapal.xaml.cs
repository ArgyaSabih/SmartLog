using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using wpf.models;

namespace wpf.modules
{
    public partial class DetailKapal : Window
    {
        private readonly long _kapalId;

        public DetailKapal(long kapalId)
        {
            InitializeComponent();
            _kapalId = kapalId;
            Loaded += DetailKapal_Loaded;
        }

        private async void DetailKapal_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDetailAsync();
        }

        private async Task LoadDetailAsync()
        {
            try
            {
                var db = App.GetService<wpf.services.PostgresService>();
                var kapal = await db.GetKapalByIdAsync(_kapalId);

                if (kapal == null)
                {
                    StyledMessageBox.ShowOk(this, "Error", "Data kapal tidak ditemukan.");
                    this.Close();
                    return;
                }

                // Header
                txtNamaKapal.Text = kapal.NamaKapal ?? "-";

                // Determine displayed status: if kapal berada di tujuan, show "Verified"
                string lokasiSekarang = kapal.LokasiSekarang ?? string.Empty;
                string lokasiTujuan = kapal.LokasiTujuan ?? string.Empty;

                string statusText;
                if (!string.IsNullOrWhiteSpace(lokasiSekarang) &&
                    !string.IsNullOrWhiteSpace(lokasiTujuan) &&
                    string.Equals(lokasiSekarang.Trim(), lokasiTujuan.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    statusText = "Verified";
                }
                else
                {
                    statusText = kapal.StatusVerifikasi ?? "Pending";
                }

                txtStatus.Text = statusText;

                // Status color mapping (based on displayed status)
                switch (statusText?.ToLower())
                {
                    case "verified":
                        borderStatus.Background = new System.Windows.Media.SolidColorBrush(
                            (System.Windows.Media.Color)
                                System.Windows.Media.ColorConverter.ConvertFromString("#90EE90")
                        ); // Light green
                        break;
                    case "pending":
                        borderStatus.Background = new System.Windows.Media.SolidColorBrush(
                            (System.Windows.Media.Color)
                                System.Windows.Media.ColorConverter.ConvertFromString("#FFD700")
                        ); // Gold
                        break;
                    case "rejected":
                        borderStatus.Background = new System.Windows.Media.SolidColorBrush(
                            (System.Windows.Media.Color)
                                System.Windows.Media.ColorConverter.ConvertFromString("#FFA07A")
                        ); // Light salmon
                        break;
                    default:
                        borderStatus.Background = new System.Windows.Media.SolidColorBrush(
                            (System.Windows.Media.Color)
                                System.Windows.Media.ColorConverter.ConvertFromString("#D3D3D3")
                        ); // Light gray
                        break;
                }

                // Information
                txtIdKapal.Text = kapal.KapalId.ToString();
                txtNomorRegistrasi.Text = kapal.KodeRegistrasi ?? kapal.NomorRegistrasi.ToString();
                txtKapasitas.Text = $"{kapal.KapasitasTon:N0} Ton";
                txtLokasiSekarang.Text = kapal.LokasiSekarang ?? "-";
                txtLokasiTujuan.Text = kapal.LokasiTujuan ?? "-";
                txtTipeKapal.Text = kapal.GetTipeKapal();

                // Biaya operasional (asumsi 1 hari)
                decimal biayaPerHari = kapal.CalculateBiayaOperasional(1);
                txtBiayaOperasional.Text = $"Biaya Operasional: Rp {biayaPerHari:N0}/hari";

                // Ketersediaan
                bool tersedia = kapal.IsAvailable();
                txtKetersediaan.Text = tersedia ? "Status: Tersedia" : "Status: Tidak Tersedia";
            }
            catch (Exception ex)
            {
                StyledMessageBox.ShowOk(this, "Error", $"Gagal memuat detail kapal: {ex.Message}");
                this.Close();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
