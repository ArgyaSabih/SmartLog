using System;
using System.Windows;
using System.Collections.ObjectModel;

namespace wpf.modules
{
    public partial class AdminView : Window
    {
        // Collection untuk menyimpan data customer
        public ObservableCollection<CustomerData> Customers { get; set; } = new ObservableCollection<CustomerData>();
        
        // Collection untuk menyimpan data kapal
        public ObservableCollection<KapalData> Kapals { get; set; } = new ObservableCollection<KapalData>();

        public AdminView()
        {
            InitializeComponent();
            
            // Set ItemsSource untuk DataGrid
            CustomerList.ItemsSource = Customers;
            KapalList.ItemsSource = Kapals;
            
            // Initialize dengan data dummy
            LoadCustomerData();
            LoadKapalData();
        }

        // Load data customer (dummy data untuk saat ini)
        private void LoadCustomerData()
        {
            Customers.Clear();
            
            Customers.Add(new CustomerData 
            { 
                Username = "rizky_anto", 
                Email = "rizky.anto21@gmail.com", 
                CustomerId = "CUST001", 
                Address = "Jl. Merpati No. 12, Jakarta",
                Phone = "081234567890",
                Date = new DateTime(2025, 7, 20)
            });
            
            Customers.Add(new CustomerData 
            { 
                Username = "bunga.sari", 
                Email = "bunga.sari88@gmail.com", 
                CustomerId = "CUST002", 
                Address = "Jl. Melati No. 45, Bandung",
                Phone = "082198765432",
                Date = new DateTime(2025, 8, 15)
            });
            
            Customers.Add(new CustomerData 
            { 
                Username = "dwi_pratama", 
                Email = "dwi.pratama77@yahoo.com", 
                CustomerId = "CUST003", 
                Address = "Perum Griya Asri Blok B2, Surabaya",
                Phone = "083156789012",
                Date = new DateTime(2025, 7, 8)
            });
            
            Customers.Add(new CustomerData 
            { 
                Username = "andika_her", 
                Email = "andika.hermawan@mail.id", 
                CustomerId = "CUST004", 
                Address = "Jl. Kenanga No. 67, Yogyakarta",
                Phone = "085234567891",
                Date = new DateTime(2025, 9, 28)
            });
            
            Customers.Add(new CustomerData 
            { 
                Username = "lina_utama", 
                Email = "lina.utama23@outlook.com", 
                CustomerId = "CUST005", 
                Address = "Jl. Anggrek No. 89, Medan",
                Phone = "083145678921",
                Date = new DateTime(2025, 5, 18)
            });
            
            Customers.Add(new CustomerData 
            { 
                Username = "johan_sett", 
                Email = "johan.setiawan@gmail.com", 
                CustomerId = "CUST006", 
                Address = "Komp. Puri Indah Blok C5, Makassar",
                Phone = "081876543210",
                Date = new DateTime(2025, 10, 12)
            });
            
            Customers.Add(new CustomerData 
            { 
                Username = "maya_putri", 
                Email = "maya.putri99@gmail.com", 
                CustomerId = "CUST007", 
                Address = "Jl. Mawar No. 10, Semarang",
                Phone = "082212345678",
                Date = new DateTime(2025, 4, 24)
            });
            
            Customers.Add(new CustomerData 
            { 
                Username = "farhan.akbar", 
                Email = "farhan.akbar11@yahoo.co.id", 
                CustomerId = "CUST008", 
                Address = "Jl. Sudirman No. 102, Balikpapan",
                Phone = "085345678912",
                Date = new DateTime(2025, 11, 30)
            });
            
            Customers.Add(new CustomerData 
            { 
                Username = "siti_nur", 
                Email = "siti.nurhaliza@mail.id", 
                CustomerId = "CUST009", 
                Address = "Jl. Cempaka No. 55, Malang",
                Phone = "081923456789",
                Date = new DateTime(2025, 1, 16)
            });
            
            Customers.Add(new CustomerData 
            { 
                Username = "aditya_ram", 
                Email = "aditya.rama88@gmail.com", 
                CustomerId = "CUST010", 
                Address = "Jl. Diponegoro No. 77, Denpasar",
                Phone = "082334567890",
                Date = new DateTime(2025, 3, 5)
            });

            // Update total count
            txtTotal.Text = Customers.Count.ToString();
        }

        // Event handler untuk tab Customer
        private void BtnCustomerTab_Click(object sender, RoutedEventArgs e)
        {
            // Set active style
            btnCustomerTab.Style = (System.Windows.Style)FindResource("ActiveTabButtonStyle");
            btnKapalTab.Style = (System.Windows.Style)FindResource("TabButtonStyle");

            // Show customer content, hide kapal content
            CustomerContent.Visibility = Visibility.Visible;
            KapalContent.Visibility = Visibility.Collapsed;
        }

        // Event handler untuk tab Kapal
        private void BtnKapalTab_Click(object sender, RoutedEventArgs e)
        {
            // Set active style
            btnKapalTab.Style = (System.Windows.Style)FindResource("ActiveTabButtonStyle");
            btnCustomerTab.Style = (System.Windows.Style)FindResource("TabButtonStyle");

            // Show kapal content, hide customer content
            CustomerContent.Visibility = Visibility.Collapsed;
            KapalContent.Visibility = Visibility.Visible;
        }

        // Load data kapal (dummy data untuk saat ini)
        private void LoadKapalData()
        {
            Kapals.Clear();
            
            Kapals.Add(new KapalData 
            { 
                NamaKapal = "Nusantara Jaya", 
                IdKapal = "KPL001", 
                NomorRegistrasi = "REG-IND-2023-001", 
                Kapasitas = "5000 Ton",
                Tujuan = "Surabaya"
            });
            
            Kapals.Add(new KapalData 
            { 
                NamaKapal = "Samudra Indah", 
                IdKapal = "KPL002", 
                NomorRegistrasi = "REG-IND-2023-002", 
                Kapasitas = "4200 Ton",
                Tujuan = "Makassar"
            });
            
            Kapals.Add(new KapalData 
            { 
                NamaKapal = "Lautan Perkasa", 
                IdKapal = "KPL003", 
                NomorRegistrasi = "REG-IND-2023-003", 
                Kapasitas = "6000 Ton",
                Tujuan = "Medan"
            });
            
            Kapals.Add(new KapalData 
            { 
                NamaKapal = "Merdeka Laut", 
                IdKapal = "KPL004", 
                NomorRegistrasi = "REG-IND-2023-004", 
                Kapasitas = "3500 Ton",
                Tujuan = "Pontianak"
            });
            
            Kapals.Add(new KapalData 
            { 
                NamaKapal = "Sinar Bahari", 
                IdKapal = "KPL005", 
                NomorRegistrasi = "REG-IND-2023-005", 
                Kapasitas = "2800 Ton",
                Tujuan = "Padang"
            });
            
            Kapals.Add(new KapalData 
            { 
                NamaKapal = "Angin Timur", 
                IdKapal = "KPL006", 
                NomorRegistrasi = "REG-IND-2023-006", 
                Kapasitas = "7000 Ton",
                Tujuan = "Jayapura"
            });
            
            Kapals.Add(new KapalData 
            { 
                NamaKapal = "Bintang Laut", 
                IdKapal = "KPL007", 
                NomorRegistrasi = "REG-IND-2023-007", 
                Kapasitas = "4500 Ton",
                Tujuan = "Ambon"
            });
            
            Kapals.Add(new KapalData 
            { 
                NamaKapal = "Cahaya Samudra", 
                IdKapal = "KPL008", 
                NomorRegistrasi = "REG-IND-2023-008", 
                Kapasitas = "5200 Ton",
                Tujuan = "Banjarmasin"
            });
            
            Kapals.Add(new KapalData 
            { 
                NamaKapal = "Harapan Baru", 
                IdKapal = "KPL009", 
                NomorRegistrasi = "REG-IND-2023-009", 
                Kapasitas = "3000 Ton",
                Tujuan = "Denpasar"
            });
            
            Kapals.Add(new KapalData 
            { 
                NamaKapal = "Laut Nusantara", 
                IdKapal = "KPL010", 
                NomorRegistrasi = "REG-IND-2023-010", 
                Kapasitas = "8000 Ton",
                Tujuan = "Manado"
            });

            // Update total count
            txtTotalKapal.Text = Kapals.Count.ToString();
        }

        // Event handler untuk logout
        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", 
                                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                // Buka LoginView
                LoginView loginView = new LoginView();
                loginView.Show();
                
                // Tutup AdminView
                this.Close();
            }
        }

        // Event handler untuk ketika kapal dipilih (single click)
        private void KapalList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (KapalList.SelectedItem is KapalData selectedKapal)
            {
                // Buka KapalDetailView (pindah page, bukan popup)
                KapalDetailView detailView = new KapalDetailView(selectedKapal, this);
                detailView.Show();
                
                // Tutup AdminView
                this.Close();
            }
        }

        // Public method untuk switch ke tab Kapal (dipanggil dari KapalDetailView)
        public void SwitchToKapalTab()
        {
            // Set active style
            btnKapalTab.Style = (System.Windows.Style)FindResource("ActiveTabButtonStyle");
            btnCustomerTab.Style = (System.Windows.Style)FindResource("TabButtonStyle");

            // Show kapal content, hide customer content
            CustomerContent.Visibility = Visibility.Collapsed;
            KapalContent.Visibility = Visibility.Visible;
        }
    }

    // Model untuk data customer
    public class CustomerData
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    // Model untuk data kapal
    public class KapalData
    {
        public string NamaKapal { get; set; } = string.Empty;
        public string IdKapal { get; set; } = string.Empty;
        public string NomorRegistrasi { get; set; } = string.Empty;
        public string Kapasitas { get; set; } = string.Empty;
        public string Tujuan { get; set; } = string.Empty;
    }
}
