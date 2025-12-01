using System;
using System.Windows;
using System.Windows.Media;

namespace wpf.modules
{
    public partial class DetailCustomer : Window
    {
        private long _pengirimanId;

        public DetailCustomer(long pengirimanId)
        {
            InitializeComponent();
            _pengirimanId = pengirimanId;
            LoadDetailAsync();
        }

        private async void LoadDetailAsync()
        {
            try
            {
                var db = App.GetService<wpf.services.PostgresService>();
                var pengiriman = await db.GetPengirimanByIdAsync(_pengirimanId);

                if (pengiriman != null)
                {
                    // Display ID
                    string displayId;
                    if (!string.IsNullOrEmpty(pengiriman.DeskripsiBarang))
                    {
                        var parts = pengiriman.DeskripsiBarang.Split('|');
                        var code = parts.Length > 0 ? parts[0] : pengiriman.DeskripsiBarang;
                        displayId = code.StartsWith("#") ? code : "#" + code;
                    }
                    else
                    {
                        displayId = "#" + pengiriman.PengirimanId.ToString();
                    }
                    txtIdBarang.Text = displayId;

                    // Nama Barang
                    txtNamaBarang.Text = pengiriman.NamaBarang ?? "-";

                    // Berat
                    txtBerat.Text = pengiriman.BeratKg.ToString("N2") + " Kg";

                    // Kapal
                    try
                    {
                        if (pengiriman.KapalId != 0)
                        {
                            var kapal = await db.GetKapalByIdAsync(pengiriman.KapalId);
                            if (kapal != null)
                            {
                                txtNamaKapal.Text = kapal.NamaKapal ?? "-";
                                txtLokasiAsal.Text = kapal.LokasiSekarang ?? "-";
                                txtLokasiTujuan.Text = kapal.LokasiTujuan ?? "-";
                            }
                            else
                            {
                                txtNamaKapal.Text = "-";
                                txtLokasiAsal.Text = "-";
                                txtLokasiTujuan.Text = "-";
                            }
                        }
                        else
                        {
                            txtNamaKapal.Text = "-";
                            txtLokasiAsal.Text = "-";
                            txtLokasiTujuan.Text = "-";
                        }
                    }
                    catch
                    {
                        txtNamaKapal.Text = "-";
                        txtLokasiAsal.Text = "-";
                        txtLokasiTujuan.Text = "-";
                    }

                    // Tanggal
                    txtTanggalDikirim.Text = pengiriman
                        .TanggalMulai.ToLocalTime()
                        .ToString("dd MMMM yyyy");
                    txtEstimasi.Text = pengiriman
                        .TanggalSelesaiEstimasi.ToLocalTime()
                        .ToString("dd MMMM yyyy");

                    // Lokasi Saat Ini
                    txtLokasiSekarang.Text = pengiriman.LokasiSaatIni ?? "-";

                    // Status
                    string status = pengiriman.StatusPengiriman ?? "Pending";
                    txtStatus.Text = status;

                    // Set status color
                    if (status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    {
                        borderStatus.Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#90EE90")
                        );
                    }
                    else if (
                        status.Equals("Proses", StringComparison.OrdinalIgnoreCase)
                        || status.Equals("Diproses", StringComparison.OrdinalIgnoreCase)
                    )
                    {
                        borderStatus.Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#FFD700")
                        );
                    }
                    else if (status.Equals("Ditunda", StringComparison.OrdinalIgnoreCase))
                    {
                        borderStatus.Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#FFA07A")
                        );
                    }
                    else if (status.Equals("Selesai", StringComparison.OrdinalIgnoreCase))
                    {
                        borderStatus.Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#90EE90")
                        );
                    }
                    else
                    {
                        borderStatus.Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#6C757D")
                        );
                    }

                    // Deskripsi - ambil bagian setelah | jika ada
                    if (!string.IsNullOrEmpty(pengiriman.DeskripsiBarang))
                    {
                        var parts = pengiriman.DeskripsiBarang.Split('|');
                        if (parts.Length > 1)
                        {
                            txtDeskripsi.Text = parts[1];
                        }
                        else
                        {
                            txtDeskripsi.Text = pengiriman.DeskripsiBarang;
                        }
                    }
                    else
                    {
                        txtDeskripsi.Text = "-";
                    }

                    // Biaya (menggunakan method dari model)
                    decimal biaya = pengiriman.CalculateCost();
                    txtBiaya.Text = "Rp " + biaya.ToString("N0");
                }
                else
                {
                    StyledMessageBox.ShowOk(this, "Error", "Data pengiriman tidak ditemukan.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                StyledMessageBox.ShowOk(this, "Error", $"Gagal memuat detail: {ex.Message}");
                this.Close();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
