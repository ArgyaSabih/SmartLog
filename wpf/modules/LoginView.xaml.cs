using System;
using System.Windows;
using System.Windows.Input; // Diperlukan untuk MouseLeftButtonDown
using System.Windows.Controls;


// Pastikan namespace ini sesuai dengan proyek Anda
namespace wpf.modules
{
    public partial class LoginView : Window
    {
        private readonly wpf.services.PostgresService _dbService;

        public LoginView()
        {
            InitializeComponent();

            // Ambil service dari DI container
            _dbService = App.GetService<wpf.services.PostgresService>();

            // Tambahkan event handler ini agar window bisa di-drag
            this.MouseLeftButtonDown += OnMouseLeftButtonDown;

            // Trik untuk watermark PasswordBox
            txtPassword.GotFocus += TxtPassword_GotFocus;
            txtPassword.LostFocus += TxtPassword_LostFocus;
            TxtPassword_LostFocus(null, null);
        }

        // --- METODE UNTUK TOMBOL ---

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
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
            
            try
            {
                // Coba login sebagai Customer
                var customer = await _dbService.LoginCustomerAsync(email, password);
                if (customer != null)
                {
                    MessageBox.Show($"Login Customer Berhasil!\n\n{customer.GetUserInfo()}", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Simpan session sederhana ke Application.Properties
                    Application.Current.Properties["CurrentUserRole"] = "Customer";
                    Application.Current.Properties["CurrentUserEmail"] = customer.Email;
                    Application.Current.Properties["CurrentUserId"] = customer.CustomerId;

                    // Buka CustomerView
                    var customerView = new CustomerView();
                    customerView.Show();
                    this.Close();
                    return;
                }

                // Coba login sebagai Admin
                var admin = await _dbService.LoginAdminAsync(email, password);
                if (admin != null)
                {
                    MessageBox.Show($"Login Admin Berhasil!\n\n{admin.GetUserInfo()}", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Simpan session sederhana ke Application.Properties
                    Application.Current.Properties["CurrentUserRole"] = "Admin";
                    Application.Current.Properties["CurrentUserEmail"] = admin.Email;
                    Application.Current.Properties["CurrentUserId"] = admin.AdminId;

                    var adminView = new AdminView();
                    adminView.Show();
                    this.Close();
                    return;
                }

                MessageBox.Show("Email atau password salah.", "Login Gagal", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Terjadi kesalahan saat login: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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