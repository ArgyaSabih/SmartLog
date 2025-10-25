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
            LoadDummyData();
            
            // Bind data ke ItemsControl
            itemsControlPengiriman.ItemsSource = DaftarPengiriman;
            
            // Set nama user (bisa diambil dari session/parameter)
            txtUserNameHeader.Text = "SmartLog";
            txtUserEmail.Text = "smartlog@gmail.com";
        }

        private void LoadDummyData()
        {
            // Data dummy sesuai dengan gambar
            DaftarPengiriman.Add(new PengirimanItem
            {
                NamaBarang = "Borax Semenko",
                IdBarang = "#nuul29b23e",
                Berat = "15 Ton",
                NamaKapal = "Nusantara Jaya",
                TanggalDibuat = "15 Jul 2025",
                Status = "Selesai",
                StatusColor = "#90EE90",
                Estimasi = "20 Jul 2025",
                Detail = "View"
            });

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
            MessageBox.Show("Fitur Buat Pesanan akan segera hadir!", 
                          "Info", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Information);
            
            // TODO: Implement form untuk membuat pesanan baru
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            // Reload data
            DaftarPengiriman.Clear();
            LoadDummyData();
            
            MessageBox.Show("Data berhasil di-refresh!", 
                          "Info", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Information);
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
