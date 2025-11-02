using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using wpf.models;
using System.Threading.Tasks;
using System.Linq;

namespace wpf.modules
{
    public partial class KapalDetailView : Window
    {
        public ObservableCollection<BarangData> Barangs { get; set; } = new ObservableCollection<BarangData>();
    private KapalData _kapalData;
    private AdminView? _parentAdminView;
    private Kapal? _kapalModel;

        public KapalDetailView(KapalData kapal, AdminView? parentView = null)
        {
            InitializeComponent();
            _kapalData = kapal;
            _parentAdminView = parentView;

            // Set kapal info from provided data immediately
            txtKapalName.Text = kapal.NamaKapal;
            txtCapacity.Text = kapal.Kapasitas.Replace(" Ton", "");
            // show unknown current location until we load real data; show tujuan from kapal data
            txtLokasiSekarang.Text = "-";
            txtTujuan.Text = kapal.Tujuan ?? "-";

            // Load full kapal model from DB asynchronously
            _ = LoadKapalFromDbAsync();

            // Set ItemsSource untuk DataGrid
            BarangList.ItemsSource = Barangs;

            // Load barang data
            LoadBarangData();
        }

        private async Task LoadKapalFromDbAsync()
        {
            try
            {
                var db = App.GetService<wpf.services.PostgresService>();
                if (long.TryParse(_kapalData.IdKapal, out long id))
                {
                    _kapalModel = await db.GetKapalByIdAsync(id);
                }

                if (_kapalModel != null)
                {
                    // Update UI with real DB values
                    txtKapalName.Dispatcher.Invoke(() => txtKapalName.Text = _kapalModel.NamaKapal ?? _kapalData.NamaKapal);
                    txtCapacity.Dispatcher.Invoke(() => txtCapacity.Text = _kapalModel.KapasitasTon.ToString("G"));
                    txtTujuan.Dispatcher.Invoke(() => txtTujuan.Text = _kapalModel.LokasiTujuan ?? "-");
                    txtLokasiSekarang.Dispatcher.Invoke(() => txtLokasiSekarang.Text = _kapalModel.LokasiSekarang ?? "-");

                    // Set verification ComboBox to current status
                    cmbStatus.Dispatcher.Invoke(() =>
                    {
                        var status = _kapalModel.StatusVerifikasi ?? "Pending";
                        // Try to select matching ComboBoxItem
                        foreach (var item in cmbStatus.Items)
                        {
                            if (item is System.Windows.Controls.ComboBoxItem cbi && string.Equals((cbi.Content as string) ?? string.Empty, status, StringComparison.OrdinalIgnoreCase))
                            {
                                cmbStatus.SelectedItem = cbi;
                                break;
                            }
                        }
                        btnSetStatus.IsEnabled = true;
                        // Update status badge
                        UpdateStatusBadge(status);
                    });
                }
            }
            catch { /* ignore and keep existing display */ }
        }

        private void UpdateStatusBadge(string? status)
        {
            var s = (status ?? "Pending").Trim();
            // Update UI on dispatcher
            statusBadge.Dispatcher.Invoke(() =>
            {
                txtStatusBadge.Text = s;
                switch (s.ToLowerInvariant())
                {
                    case "verified":
                        statusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1F4E0"));
                        txtStatusBadge.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F7A3A"));
                        break;
                    case "rejected":
                        statusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8D7DA"));
                        txtStatusBadge.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7A1A1A"));
                        break;
                    default:
                        statusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF4C2"));
                        txtStatusBadge.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8A6D00"));
                        break;
                }
            });
        }

        private void LoadBarangData()
        {
            Barangs.Clear();

            // Data dummy untuk barang
            Barangs.Add(new BarangData
            {
                IdBarang = "#BRG001",
                NamaBarang = "Beras Premium",
                BeratBarang = "15 Ton",
                Customer = "Rizky Anto",
                TanggalMasuk = new DateTime(2025, 9, 12)
            });

            Barangs.Add(new BarangData
            {
                IdBarang = "#BRG002",
                NamaBarang = "Gula Pasir",
                BeratBarang = "10 Ton",
                Customer = "Bunga Sari",
                TanggalMasuk = new DateTime(2025, 9, 13)
            });

            Barangs.Add(new BarangData
            {
                IdBarang = "#BRG003",
                NamaBarang = "Minyak Goreng",
                BeratBarang = "8 Ton",
                Customer = "Dwi Pratama",
                TanggalMasuk = new DateTime(2025, 9, 14)
            });

            Barangs.Add(new BarangData
            {
                IdBarang = "#BRG004",
                NamaBarang = "Kopi Robusta",
                BeratBarang = "12 Ton",
                Customer = "Andika Hermawan",
                TanggalMasuk = new DateTime(2025, 9, 15)
            });

            Barangs.Add(new BarangData
            {
                IdBarang = "#BRG005",
                NamaBarang = "Teh Hijau",
                BeratBarang = "20 Ton",
                Customer = "Lina Utama",
                TanggalMasuk = new DateTime(2025, 9, 16)
            });

            Barangs.Add(new BarangData
            {
                IdBarang = "#BRG006",
                NamaBarang = "Garam Halus",
                BeratBarang = "18 Ton",
                Customer = "Johan Setiawan",
                TanggalMasuk = new DateTime(2025, 9, 17)
            });
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Buka AdminView dengan tab Kapal aktif
            AdminView adminView = new AdminView();
            adminView.Show();
            adminView.SwitchToKapalTab(); // Panggil method untuk switch ke tab Kapal
            
            // Tutup detail view
            this.Close();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            // Konfirmasi logout
            var result = MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", 
                                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                // Kembali ke LoginView
                LoginView loginView = new LoginView();
                loginView.Show();
                
                // Tutup detail view
                this.Close();
            }
        }

        // Show location details (from DB if available)
        private void BtnDetailLokasi_Click(object sender, RoutedEventArgs e)
        {
            string lokasi = _kapalModel?.LokasiSekarang ?? txtLokasiSekarang.Text;
            string message = $"Lokasi saat ini:\n{lokasi}\n\nStatus Verifikasi: {_kapalModel?.StatusVerifikasi ?? "Unknown"}";
            MessageBox.Show(message, "Detail Lokasi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Update lokasi via small dialog and persist to DB
        private async void BtnUpdateLokasi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string current = _kapalModel?.LokasiSekarang ?? txtLokasiSekarang.Text;
                var dlg = new UpdateLocationDialog(current);
                dlg.Owner = this;
                var res = dlg.ShowDialog();
                if (res == true && !string.IsNullOrWhiteSpace(dlg.LocationResult))
                {
                    var newLoc = dlg.LocationResult!;
                    // Confirm update
                    var confirm = MessageBox.Show($"Update lokasi kapal dari '{_kapalModel?.LokasiSekarang ?? txtLokasiSekarang.Text}' ke '{newLoc}'?", "Konfirmasi Update Lokasi", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (confirm != MessageBoxResult.Yes) return;
                    // Update model
                    if (_kapalModel == null)
                    {
                        // try to load minimal model to update
                        var dbx = App.GetService<wpf.services.PostgresService>();
                        if (long.TryParse(_kapalData.IdKapal, out long id))
                        {
                            _kapalModel = await dbx.GetKapalByIdAsync(id);
                        }
                    }

                    if (_kapalModel != null)
                    {
                        _kapalModel.UpdateLokasi(newLoc);
                        var db = App.GetService<wpf.services.PostgresService>();
                        await db.UpdateKapalAsync(_kapalModel);

                        // Update UI
                        txtLokasiSekarang.Text = _kapalModel.LokasiSekarang ?? newLoc;

                        MessageBox.Show("Lokasi kapal berhasil diperbarui.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Gagal menemukan data kapal untuk diupdate.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal mengupdate lokasi: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Removed BtnLihatKapasitas per request

        private void BtnSetStatus_Click(object sender, RoutedEventArgs e)
        {
            // Open popup for status selection. Ensure current status is selected in combo.
            try
            {
                var status = _kapalModel?.StatusVerifikasi ?? "Pending";

                // select matching item if available
                foreach (var item in cmbStatus.Items)
                {
                    if (item is System.Windows.Controls.ComboBoxItem cbi && string.Equals((cbi.Content as string) ?? string.Empty, status, StringComparison.OrdinalIgnoreCase))
                    {
                        cmbStatus.SelectedItem = cbi;
                        break;
                    }
                }

                popupStatus.IsOpen = true;
            }
            catch
            {
                // if anything goes wrong just open the popup
                popupStatus.IsOpen = true;
            }
        }

        private void BtnCancelStatus_Click(object sender, RoutedEventArgs e)
        {
            popupStatus.IsOpen = false;
        }

        private async void BtnApplyStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_kapalModel == null)
                {
                    var dbx = App.GetService<wpf.services.PostgresService>();
                    if (long.TryParse(_kapalData.IdKapal, out long id))
                    {
                        _kapalModel = await dbx.GetKapalByIdAsync(id);
                    }
                }

                if (_kapalModel == null)
                {
                    MessageBox.Show("Gagal menemukan data kapal.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedStatus = "Pending";
                if (cmbStatus.SelectedItem is System.Windows.Controls.ComboBoxItem cbi)
                    selectedStatus = cbi.Content as string ?? "Pending";

                if (string.Equals(_kapalModel.StatusVerifikasi ?? "Pending", selectedStatus, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Status tidak berubah.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    popupStatus.IsOpen = false;
                    return;
                }

                var result = MessageBox.Show($"Ubah status verifikasi kapal dari '{_kapalModel.StatusVerifikasi ?? "Pending"}' menjadi '{selectedStatus}'?", "Konfirmasi", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                _kapalModel.StatusVerifikasi = selectedStatus;
                var db = App.GetService<wpf.services.PostgresService>();
                await db.UpdateKapalAsync(_kapalModel);

                // Refresh badge immediately
                UpdateStatusBadge(selectedStatus);

                MessageBox.Show("Status verifikasi kapal berhasil diperbarui.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);

                popupStatus.IsOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal mengubah status verifikasi: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // Model untuk data barang
    public class BarangData
    {
        public string IdBarang { get; set; } = string.Empty;
        public string NamaBarang { get; set; } = string.Empty;
        public string BeratBarang { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public DateTime TanggalMasuk { get; set; }
    }
}
