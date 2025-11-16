using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using wpf.models;

namespace wpf.modules
{
    public partial class KapalDetailView : Window
    {
        public ObservableCollection<BarangData> Barangs { get; set; } =
            new ObservableCollection<BarangData>();
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
            // Try to convert Kapasitas (e.g. "3 Ton") to kilograms for display
            try
            {
                var raw = kapal.Kapasitas?.Replace(" Ton", "")?.Trim() ?? string.Empty;
                if (decimal.TryParse(raw, out decimal ton))
                {
                    txtCapacity.Text = (ton * 1000m).ToString("G");
                }
                else
                {
                    txtCapacity.Text = kapal.Kapasitas.Replace(" Ton", "");
                }
            }
            catch
            {
                txtCapacity.Text = kapal.Kapasitas.Replace(" Ton", "");
            }
            // show unknown current location until we load real data; show tujuan from kapal data
            txtLokasiSekarang.Text = "-";
            txtTujuan.Text = $"Tujuan: {kapal.Tujuan ?? "-"}";

            // Start async initialization (loads kapal and barang)
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            await LoadKapalFromDbAsync();
            // Ensure ItemsSource is set on UI thread
            this.Dispatcher.Invoke(() => BarangList.ItemsSource = Barangs);
            await LoadBarangDataFromDbAsync();
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
                    txtKapalName.Dispatcher.Invoke(
                        () => txtKapalName.Text = _kapalModel.NamaKapal ?? _kapalData.NamaKapal
                    );
                    txtCapacity.Dispatcher.Invoke(
                        () => txtCapacity.Text = (_kapalModel.KapasitasTon * 1000m).ToString("G")
                    );
                    txtTujuan.Dispatcher.Invoke(
                        () => txtTujuan.Text = $"tujuan: {_kapalModel.LokasiTujuan ?? "-"}"
                    );
                    txtLokasiSekarang.Dispatcher.Invoke(
                        () => txtLokasiSekarang.Text = _kapalModel.LokasiSekarang ?? "-"
                    );
                    // Update status badge (no in-place status editor)
                    UpdateStatusBadge(_kapalModel.StatusVerifikasi ?? "Pending");
                }
            }
            catch
            { /* ignore and keep existing display */
            }
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
                        statusBadge.Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#D1F4E0")
                        );
                        txtStatusBadge.Foreground = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#1F7A3A")
                        );
                        break;
                    case "rejected":
                        statusBadge.Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#F8D7DA")
                        );
                        txtStatusBadge.Foreground = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#7A1A1A")
                        );
                        break;
                    default:
                        statusBadge.Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#FFF4C2")
                        );
                        txtStatusBadge.Foreground = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#8A6D00")
                        );
                        break;
                }
            });
        }

        private async Task LoadBarangDataFromDbAsync()
        {
            try
            {
                var db = App.GetService<wpf.services.PostgresService>();
                if (_kapalModel == null)
                {
                    if (long.TryParse(_kapalData.IdKapal, out long id))
                    {
                        _kapalModel = await db.GetKapalByIdAsync(id);
                    }
                }

                if (_kapalModel == null)
                    return;

                // Get pengirimans that belong to this kapal
                var pengirimans = await db.GetPengirimansByKapalIdAsync(_kapalModel.KapalId);

                // Build local list on worker thread
                var items = new System.Collections.Generic.List<BarangData>();
                foreach (var p in pengirimans)
                {
                    items.Add(
                        new BarangData
                        {
                            PengirimanId = p.PengirimanId,
                            IdBarang = $"#P{p.PengirimanId}",
                            NamaBarang = p.NamaBarang ?? "-",
                            BeratBarang = FormatBeratKgToDisplay(p.BeratKg),
                            BeratKgValue = p.BeratKg,
                            Customer = p.CustomerId.ToString(),
                            TanggalMasuk = p.TanggalMulai,
                            Status = p.StatusPengiriman ?? "Pending",
                        }
                    );
                }

                // Update ObservableCollection on UI thread
                this.Dispatcher.Invoke(() =>
                {
                    Barangs.Clear();
                    foreach (var it in items)
                        Barangs.Add(it);
                    UpdateCapacityDisplay();
                });
            }
            catch (Exception ex)
            {
                // Keep UI responsive; show simple message
                MessageBox.Show(
                    $"Gagal memuat data barang: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private string FormatBeratKgToDisplay(decimal beratKg)
        {
            // If >= 1000 kg, show in tons with 3 decimal precision
            if (beratKg >= 1000)
            {
                decimal ton = Math.Round(beratKg / 1000m, 3);
                return $"{ton} Ton";
            }
            return $"{beratKg} Kg";
        }

        private void UpdateCapacityDisplay()
        {
            if (_kapalModel == null)
                return;
            // Sum berat barang in kg for items currently in 'Proses' status only (exclude Pending)
            decimal totalKg = Barangs
                .Where(b => string.Equals(b.Status, "Proses", StringComparison.OrdinalIgnoreCase))
                .Sum(b => b.BeratKgValue);
            // kapal kapasitas in ton -> convert to kg
            decimal kapasitasKg = _kapalModel.KapasitasTon * 1000m;
            decimal sisaKg = kapasitasKg - totalKg;
            if (sisaKg < 0)
                sisaKg = 0;
            // show in kilograms
            txtCapacity.Text = sisaKg.ToString("G");
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
            var result = MessageBox.Show(
                "Apakah Anda yakin ingin logout?",
                "Konfirmasi Logout",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                // Kembali ke LoginView
                LoginView loginView = new LoginView();
                loginView.Show();

                // Tutup detail view
                this.Close();
            }
        }

        // Event handler untuk tombol detail barang
        private void BtnDetailBarang_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag != null)
            {
                if (long.TryParse(fe.Tag.ToString(), out long pengirimanId))
                {
                    var detailDialog = new DetailPengirimanKapal(pengirimanId);
                    detailDialog.Owner = this;
                    detailDialog.ShowDialog();
                }
            }
        }

        // Show location details (from DB if available) and fetch weather from weatherapi.com
        private async void BtnDetailLokasi_Click(object sender, RoutedEventArgs e)
        {
            string lokasi = _kapalModel?.LokasiSekarang ?? txtLokasiSekarang.Text;
            string kota = (lokasi ?? string.Empty).Split(',')[0].Trim();

            // Read API key from environment variable `API_KEY` (ensure .env is loaded in your environment)
            string? apiKey = Environment.GetEnvironmentVariable("API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                string message =
                    $"Lokasi saat ini:\n{lokasi}\n\nStatus Verifikasi: {_kapalModel?.StatusVerifikasi ?? "Unknown"}\n\n" +
                    "API_KEY belum diset. Silakan tambahkan API_KEY Anda ke file .env atau environment variables.";
                MessageBox.Show(message, "Detail Lokasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var client = new System.Net.Http.HttpClient();
                var url = $"https://api.weatherapi.com/v1/current.json?key={System.Uri.EscapeDataString(apiKey)}&q={System.Uri.EscapeDataString(kota)}&aqi=no";
                var resp = await client.GetAsync(url);
                if (!resp.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Gagal mengambil data cuaca: {resp.StatusCode}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using var stream = await resp.Content.ReadAsStreamAsync();
                using var doc = await System.Text.Json.JsonDocument.ParseAsync(stream);
                var root = doc.RootElement;
                var loc = root.GetProperty("location");
                var cur = root.GetProperty("current");

                string locName = loc.GetProperty("name").GetString() ?? kota;
                string region = loc.GetProperty("region").GetString() ?? string.Empty;
                string country = loc.GetProperty("country").GetString() ?? string.Empty;
                string localtime = loc.GetProperty("localtime").GetString() ?? string.Empty;
                decimal tempC = cur.GetProperty("temp_c").GetDecimal();
                string condition = cur.GetProperty("condition").GetProperty("text").GetString() ?? string.Empty;
                decimal windKph = cur.GetProperty("wind_kph").GetDecimal();
                int humidity = cur.GetProperty("humidity").GetInt32();

                string message =
                    $"Lokasi saat ini:\n{locName}{(string.IsNullOrWhiteSpace(region) ? "" : ", " + region)}{(string.IsNullOrWhiteSpace(country) ? "" : ", " + country)}\nWaktu lokal: {localtime}\n\n" +
                    $"Cuaca: {condition}\nSuhu: {tempC} °C\nKelembapan: {humidity}%\nKecepatan angin: {windKph} kph\n\n" +
                    $"Status Verifikasi: {_kapalModel?.StatusVerifikasi ?? "Unknown"}";

                MessageBox.Show(message, "Detail Lokasi & Cuaca", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal mengambil detail lokasi/cuaca: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                    // Before applying, compute affected pengirimans and total weight to show confirmation
                    var dbPreview = App.GetService<wpf.services.PostgresService>();
                    // Ensure kapal model is loaded
                    if (_kapalModel == null)
                    {
                        if (long.TryParse(_kapalData.IdKapal, out long id))
                        {
                            _kapalModel = await dbPreview.GetKapalByIdAsync(id);
                        }
                    }

                    if (_kapalModel == null)
                    {
                        MessageBox.Show("Gagal menemukan data kapal untuk pra-konfirmasi.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var pengirimansPreview = await dbPreview.GetPengirimansByKapalIdAsync(_kapalModel.KapalId);
                    var affected = pengirimansPreview.Where(p => string.Equals(p.StatusPengiriman ?? "", "Proses", StringComparison.OrdinalIgnoreCase) || string.Equals(p.StatusPengiriman ?? "", "Diproses", StringComparison.OrdinalIgnoreCase)).ToList();
                    int countAffected = affected.Count;
                    decimal totalKgAffected = affected.Sum(p => p.BeratKg);

                    bool willMarkFinished = string.Equals(newLoc?.Trim(), _kapalModel.LokasiTujuan?.Trim(), StringComparison.OrdinalIgnoreCase);

                    string curLocDisplay = _kapalModel.LokasiSekarang ?? "-";
                    string msg = $"Lokasi kapal akan diubah dari '{curLocDisplay}' menjadi '{newLoc}'.\n\n" +
                                 $"Pengiriman yang akan terpengaruh: {countAffected} barang\n" +
                                 $"Total berat yang akan dibebaskan: {totalKgAffected} Kg ({Math.Round(totalKgAffected/1000m,3)} Ton)\n\n";
                    if (willMarkFinished)
                    {
                        msg += "Karena lokasi baru sama dengan tujuan kapal, pengiriman yang diproses akan ditandai sebagai 'Selesai'.\n\n";
                    }
                    msg += "Lanjutkan update lokasi?";

                    var confirm = MessageBox.Show(msg, "Konfirmasi Update Lokasi", MessageBoxButton.YesNo, MessageBoxImage.Question);
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

                        // Refresh barang list so statuses/locations reflect cascade updates
                        await LoadBarangDataFromDbAsync();

                        // Update UI
                        txtLokasiSekarang.Text = _kapalModel.LokasiSekarang ?? newLoc;

                        MessageBox.Show(
                            "Lokasi kapal berhasil diperbarui.",
                            "Sukses",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );
                    }
                    else
                    {
                        MessageBox.Show(
                            "Gagal menemukan data kapal untuk diupdate.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Gagal mengupdate lokasi: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        // Event handler untuk verifikasi barang (ubah status jadi "Proses")
        private async void BtnVerifyBarang_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(sender is FrameworkElement fe) || !(fe.DataContext is BarangData barangData))
                    return;

                var confirm = MessageBox.Show(
                    $"Verifikasi barang '{barangData.NamaBarang}'?\n\nStatus pengiriman akan diubah menjadi 'Proses'.",
                    "Konfirmasi Verifikasi",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (confirm != MessageBoxResult.Yes)
                    return;

                var db = App.GetService<wpf.services.PostgresService>();
                var pengiriman = await db.GetPengirimanByIdAsync(barangData.PengirimanId);

                if (pengiriman != null)
                {
                    pengiriman.UpdateStatus("Proses");
                    await db.UpdatePengirimanAsync(pengiriman);

                    MessageBox.Show(
                        "Barang berhasil diverifikasi.\nStatus pengiriman diubah menjadi 'Proses'.",
                        "Sukses",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    // Reload data to reflect changes
                    await LoadBarangDataFromDbAsync();
                }
                else
                {
                    MessageBox.Show(
                        "Data pengiriman tidak ditemukan.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Gagal memverifikasi barang: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        // Event handler untuk cancel barang
        private async void BtnDeleteBarang_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(sender is FrameworkElement fe) || !(fe.DataContext is BarangData barangData))
                    return;

                var confirm = MessageBox.Show(
                    $"Batalkan pengiriman barang '{barangData.NamaBarang}'?\n\nStatus pengiriman akan diubah menjadi 'Dibatalkan'.",
                    "Konfirmasi Pembatalan",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (confirm != MessageBoxResult.Yes)
                    return;

                var db = App.GetService<wpf.services.PostgresService>();
                var pengiriman = await db.GetPengirimanByIdAsync(barangData.PengirimanId);

                if (pengiriman != null)
                {
                    pengiriman.UpdateStatus("Dibatalkan");
                    await db.UpdatePengirimanAsync(pengiriman);

                    MessageBox.Show(
                        "Pengiriman barang berhasil dibatalkan.\nStatus diubah menjadi 'Dibatalkan'.",
                        "Sukses",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                    );

                    // Reload data to reflect changes
                    await LoadBarangDataFromDbAsync();
                }
                else
                {
                    MessageBox.Show(
                        "Data pengiriman tidak ditemukan.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Gagal membatalkan pengiriman: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        // Removed BtnLihatKapasitas per request

        // Status modification removed from KapalDetailView (handled elsewhere or disabled)

        // Adding barang from Admin dashboard removed per request
    }

    // Model untuk data barang
    public class BarangData
    {
        public long PengirimanId { get; set; }
        public string IdBarang { get; set; } = string.Empty;
        public string NamaBarang { get; set; } = string.Empty;
        public string BeratBarang { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public DateTime TanggalMasuk { get; set; }

        // numeric value in kilograms for calculations
        public decimal BeratKgValue { get; set; }

        // Status properties
        public string Status { get; set; } = "Pending";
        public string StatusBackground
        {
            get
            {
                return Status?.ToLower() switch
                {
                    "proses" => "#FFF3CD",
                    "diproses" => "#FFF3CD",
                    "selesai" => "#D1F4E0",
                    "dibatalkan" => "#F8D7DA",
                    _ => "#FFE5D1", // Pending
                };
            }
        }
        public string StatusForeground
        {
            get
            {
                // Use a dark foreground for all statuses to ensure contrast over light badges
                return "#222222";
            }
        }
    }
}
