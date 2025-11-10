using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace wpf.modules
{
    public partial class CustomerView : Window
    {
        // Collection untuk menampung data pengiriman
        public ObservableCollection<PengirimanItem> DaftarPengiriman { get; set; }

        public CustomerView()
        {
            InitializeComponent();
            
            // Initialize data
            DaftarPengiriman = new ObservableCollection<PengirimanItem>();

            // Bind data ke ItemsControl
            itemsControlPengiriman.ItemsSource = DaftarPengiriman;

            // Load from DB
            LoadCustomerDataFromDbAsync();
        }

        private async void LoadCustomerDataFromDbAsync()
        {
            try
            {
                var db = App.GetService<wpf.services.PostgresService>();

                // Get current user id (customer)
                long? customerId = Application.Current.Properties.Contains("CurrentUserId") ? Application.Current.Properties["CurrentUserId"] as long? : null;
                
                // If we have customer id, fetch pengirimans for that customer
                if (customerId.HasValue)
                {
                    var pengirimans = await db.GetPengirimansByCustomerIdAsync(customerId.Value);
                    DaftarPengiriman.Clear();
                    foreach (var p in pengirimans)
                    {
                        // Determine display ID: try to read generated code from DeskripsiBarang
                        string displayId;
                        if (!string.IsNullOrEmpty(p.DeskripsiBarang))
                        {
                            // DeskripsiBarang may contain code|detail when saved by AddExportView
                            var parts = p.DeskripsiBarang.Split('|');
                            var code = parts.Length > 0 ? parts[0] : p.DeskripsiBarang;
                            displayId = code.StartsWith("#") ? code : "#" + code;
                        }
                        else
                        {
                            displayId = p.PengirimanId.ToString();
                        }

                        // Try to resolve kapal name if possible
                        string kapalName = string.Empty;
                        try
                        {
                            if (p.KapalId != 0)
                            {
                                var kapal = await db.GetKapalByIdAsync(p.KapalId);
                                kapalName = kapal != null ? kapal.NamaKapal ?? string.Empty : string.Empty;
                            }
                        }
                        catch { kapalName = string.Empty; }

                        // Map status to color (basic)
                        string status = p.StatusPengiriman ?? string.Empty;
                        string statusColor = "#90EE90"; // default green
                        if (status.Equals("Proses", StringComparison.OrdinalIgnoreCase) || status.Equals("Diproses", StringComparison.OrdinalIgnoreCase))
                            statusColor = "#FFD700"; // gold
                        else if (status.Equals("Ditunda", StringComparison.OrdinalIgnoreCase))
                            statusColor = "#FFA07A"; // light salmon
                        else if (status.Equals("Selesai", StringComparison.OrdinalIgnoreCase))
                            statusColor = "#90EE90"; // green

                        DaftarPengiriman.Add(new PengirimanItem
                        {
                            NamaBarang = p.NamaBarang ?? string.Empty,
                            IdBarang = displayId,
                            Berat = p.BeratKg.ToString() + " Kg",
                            NamaKapal = kapalName,
                            // Convert stored UTC to local time for display
                            TanggalDibuat = p.TanggalMulai.ToLocalTime().ToString("dd MMM yyyy"),
                            Status = status,
                            StatusColor = statusColor,
                            Estimasi = p.TanggalSelesaiEstimasi.ToLocalTime().ToString("dd MMM yyyy"),
                            Detail = "View"
                        });
                    }

                    // Set header info
                    var cust = await db.GetCustomerByIdAsync(customerId.Value);
                    if (cust != null)
                    {
                        txtUserNameHeader.Text = cust.Nama ?? "Customer";
                        txtUserEmail.Text = cust.Email ?? string.Empty;
                    }
                }
                else
                {
                    // fallback to dummy data if no session
                    LoadDummyData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal memuat data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadDummyData();
            }

        }

        // Event Handlers
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Apakah Anda yakin ingin logout?", 
                                        "Konfirmasi Logout", 
                                        MessageBoxButton.YesNo, 
                                        MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                // Kembali ke LoginView
                LoginView loginView = new LoginView();
                loginView.Show();
                this.Close();
            }
        }

        private void BtnBuatPesanan_Click(object sender, RoutedEventArgs e)
        {
            // Open AddExport dialog and refresh data on success
            try
            {
                var dialog = new AddExportView();
                dialog.Owner = this;
                var result = dialog.ShowDialog();
                if (result == true)
                {
                    // Refresh from DB to show newly added pengiriman
                    DaftarPengiriman.Clear();
                    LoadCustomerDataFromDbAsync();
                    MessageBox.Show("Pengiriman berhasil ditambahkan.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal membuka form tambah pengiriman: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            // Reload data from DB (or fallback dummy)
            DaftarPengiriman.Clear();
            LoadCustomerDataFromDbAsync();

            MessageBox.Show("Data berhasil di-refresh!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDetailPengiriman_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {
                string? idBarang = button.Tag.ToString();
                if (!string.IsNullOrEmpty(idBarang))
                {
                    MessageBox.Show($"Detail pengiriman untuk ID: {idBarang}\n\nFitur detail akan segera hadir!", 
                                  "Detail Pengiriman", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Information);
                }
                
                // TODO: Implement halaman detail pengiriman
            }
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Halaman Previous", "Pagination", MessageBoxButton.OK, MessageBoxImage.Information);
            // TODO: Implement pagination logic
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Halaman Next", "Pagination", MessageBoxButton.OK, MessageBoxImage.Information);
            // TODO: Implement pagination logic
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fitur Filter akan segera hadir!", 
                          "Info", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Information);
            // TODO: Implement filter functionality
        }

        // Local dummy data loader (used as fallback or for demo)
        private void LoadDummyData()
        {
            DaftarPengiriman.Clear();

            DaftarPengiriman.Add(new PengirimanItem
            {
                NamaBarang = "Kabel Listrik",
                IdBarang = "#nuul29b1",
                Berat = "10 Ton",
                NamaKapal = "Samudra Raya",
                TanggalDibuat = "13 Mei 2025",
                Status = "Selesai",
                StatusColor = "#90EE90",
                Estimasi = "19 Mei 2025",
                Detail = "View"
            });

            DaftarPengiriman.Add(new PengirimanItem
            {
                NamaBarang = "Minyak Goreng",
                IdBarang = "#nuul2c3e8",
                Berat = "25 Ton",
                NamaKapal = "Lautan Perkasa",
                TanggalDibuat = "30 Jan 2025",
                Status = "Proses",
                StatusColor = "#FFD700",
                Estimasi = "06 Jul 2025",
                Detail = "View"
            });

            DaftarPengiriman.Add(new PengirimanItem
            {
                NamaBarang = "Kopi Robusta",
                IdBarang = "#nuul34e0l",
                Berat = "8 Ton",
                NamaKapal = "Mentari Jawi",
                TanggalDibuat = "18 Sep 2025",
                Status = "Selesai",
                StatusColor = "#90EE90",
                Estimasi = "26 Sep 2025",
                Detail = "View"
            });

            DaftarPengiriman.Add(new PengirimanItem
            {
                NamaBarang = "Teh Hijau",
                IdBarang = "#nuul33d5o7",
                Berat = "12 Ton",
                NamaKapal = "Binar Emas",
                TanggalDibuat = "04 Mar 2025",
                Status = "Proses",
                StatusColor = "#FFD700",
                Estimasi = "10 Mar 2025",
                Detail = "View"
            });

            DaftarPengiriman.Add(new PengirimanItem
            {
                NamaBarang = "Garam Halus",
                IdBarang = "#nuul299e3",
                Berat = "18 Ton",
                NamaKapal = "Angin Timur",
                TanggalDibuat = "02 Okt 2025",
                Status = "Ditunda",
                StatusColor = "#FFA07A",
                Estimasi = "12 Okt 2025",
                Detail = "View"
            });

            DaftarPengiriman.Add(new PengirimanItem
            {
                NamaBarang = "Beras Bulog",
                IdBarang = "#nuul289te8",
                Berat = "22 Ton",
                NamaKapal = "Bintang Laut",
                TanggalDibuat = "14 Feb 2025",
                Status = "Selesai",
                StatusColor = "#90EE90",
                Estimasi = "24 Feb 2025",
                Detail = "View"
            });

            DaftarPengiriman.Add(new PengirimanItem
            {
                NamaBarang = "Jagung Pangan",
                IdBarang = "#nuul19l9t3",
                Berat = "30 Ton",
                NamaKapal = "Cahaya Samudera",
                TanggalDibuat = "20 Nov 2025",
                Status = "Selesai",
                StatusColor = "#90EE90",
                Estimasi = "30 Nov 2025",
                Detail = "View"
            });

            DaftarPengiriman.Add(new PengirimanItem
            {
                NamaBarang = "Kacang Tanah",
                IdBarang = "#nuul29t34",
                Berat = "9 Ton",
                NamaKapal = "Harapan Baru",
                TanggalDibuat = "07 Jan 2025",
                Status = "Ditunda",
                StatusColor = "#FFA07A",
                Estimasi = "16 Jan 2025",
                Detail = "View"
            });

            DaftarPengiriman.Add(new PengirimanItem
            {
                NamaBarang = "Tanjung Tarigu",
                IdBarang = "#nuul12t28",
                Berat = "14 Ton",
                NamaKapal = "Likid Nusantara",
                TanggalDibuat = "25 Des 2025",
                Status = "Selesai",
                StatusColor = "#90EE90",
                Estimasi = "04 Mar 2025",
                Detail = "View"
            });
        }
    }

    // Model untuk item pengiriman
    public class PengirimanItem
    {
        public string NamaBarang { get; set; } = string.Empty;
        public string IdBarang { get; set; } = string.Empty;
        public string Berat { get; set; } = string.Empty;
        public string NamaKapal { get; set; } = string.Empty;
        public string TanggalDibuat { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public string Estimasi { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }
}
