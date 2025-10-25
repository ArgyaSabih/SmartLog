using System;
using System.Windows;
using System.Collections.ObjectModel;

namespace wpf.modules
{
    public partial class KapalDetailView : Window
    {
        public ObservableCollection<BarangData> Barangs { get; set; } = new ObservableCollection<BarangData>();
        private KapalData _kapalData;
        private AdminView? _parentAdminView;

        public KapalDetailView(KapalData kapal, AdminView? parentView = null)
        {
            InitializeComponent();
            _kapalData = kapal;
            _parentAdminView = parentView;

            // Set kapal info
            txtKapalName.Text = kapal.NamaKapal;
            txtCapacity.Text = kapal.Kapasitas.Replace(" Ton", "");
            txtCoordinates.Text = "37°25'19.07''N, 122°05'06.24''E"; // Default coordinates

            // Set ItemsSource untuk DataGrid
            BarangList.ItemsSource = Barangs;

            // Load barang data
            LoadBarangData();
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
