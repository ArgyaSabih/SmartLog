using System;
using System.Windows;
using System.Windows.Input; // Diperlukan untuk MouseLeftButtonDown
using System.Windows.Controls;


// Pastikan namespace ini sesuai dengan proyek Anda
namespace wpf.modules
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
            
            // Tambahkan event handler ini agar window bisa di-drag
            this.MouseLeftButtonDown += OnMouseLeftButtonDown;
            
            // Trik untuk watermark PasswordBox
            txtPassword.GotFocus += TxtPassword_GotFocus;
            txtPassword.LostFocus += TxtPassword_LostFocus;
            TxtPassword_LostFocus(null, null);
        }

        // --- METODE UNTUK TOMBOL ---

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;

            // Validasi input
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Email dan password harus diisi!", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // DEMONSTRASI OOP: Menggunakan Polymorphism untuk login
            // Admin dan Customer punya method Login() yang sama tapi behavior berbeda
            
            // Cek apakah login sebagai Customer
            if (email == "customer@gmail.com" && password == "customer123")
            {
                // POLYMORPHISM: Customer.Login() memanggil override method
                var customer = new wpf.models.Customer 
                { 
                    CustomerId = 1, 
                    Nama = "Demo Customer"
                };
                customer.Register(email, password, "Demo Customer", "Jl. Demo No. 1", "08123456789");
                
                bool loginSuccess = customer.Login(email, password);
                if (loginSuccess)
                {
                    MessageBox.Show($"Login Customer Berhasil!\n\n{customer.GetUserInfo()}", 
                                  "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Buka CustomerView
                    CustomerView customerView = new CustomerView();
                    customerView.Show();
                    
                    // Tutup LoginView
                    this.Close();
                    return;
                }
            }
            // Cek apakah login sebagai Admin
            else if (email == "admin@gmail.com" && password == "admin123")
            {
                // POLYMORPHISM: Admin.Login() memanggil override method dengan verification check
                var admin = new wpf.models.Admin 
                { 
                    AdminId = 1, 
                    Nama = "Demo Admin",
                    Role = "Super Admin"
                };
                admin.Register(email, password, "Demo Admin", "Super Admin");
                admin.VerifyAdmin("System"); // Admin harus verified dulu
                
                bool loginSuccess = admin.Login(email, password);
                if (loginSuccess)
                {
                    MessageBox.Show($"Login Admin Berhasil!\n\n{admin.GetUserInfo()}", 
                                  "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    // Buka AdminView
                    AdminView adminView = new AdminView();
                    adminView.Show();
                    
                    // Tutup LoginView
                    this.Close();
                    return;
                }
            }
            else
            {
                MessageBox.Show("Email atau password salah.", "Login Gagal", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        
        // --- METODE BANTUAN ---

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            // Buka jendela Register dengan Dispatcher
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                RegisterView registerWindow = new RegisterView();
                registerWindow.Show();
                this.Close();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
        
        // Membuat window bisa di-drag
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Cek apakah yang diklik bukan tombol atau input
            if (e.Source == sender) 
            {
                 this.DragMove();
            }
        }

        // --- Trik untuk Watermark PasswordBox ---
        
        // Saat PasswordBox mendapat fokus, sembunyikan watermark
        private void TxtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            // Cari TextBlock Watermark di dalam Template PasswordBox
            var watermark = txtPassword.Template.FindName("Watermark", txtPassword) as TextBlock;
            if (watermark != null)
            {
                watermark.Visibility = Visibility.Collapsed;
            }
        }

        // Saat PasswordBox kehilangan fokus
        private void TxtPassword_LostFocus(object? sender, RoutedEventArgs? e)
        {
            // Cari TextBlock Watermark
            var watermark = txtPassword.Template.FindName("Watermark", txtPassword) as TextBlock;
            if (watermark != null)
            {
                // Tampilkan watermark HANYA JIKA PasswordBox kosong
                if (string.IsNullOrEmpty(txtPassword.Password))
                {
                    watermark.Visibility = Visibility.Visible;
                }
                else
                {
                    watermark.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}