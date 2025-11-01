using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace wpf.modules
{
    public partial class RegisterView : Window
    {
        private readonly wpf.services.PostgresService _dbService;

        public RegisterView()
        {
            InitializeComponent();

            // Ambil service postgres dari DI
            _dbService = App.GetService<wpf.services.PostgresService>();

            // Tambahkan event handler agar window bisa di-drag
            this.MouseLeftButtonDown += OnMouseLeftButtonDown;

            // Trik untuk watermark PasswordBox
            txtPassword.GotFocus += TxtPassword_GotFocus;
            txtPassword.LostFocus += TxtPassword_LostFocus;
            TxtPassword_LostFocus(null, null);
        }

        // --- METODE UNTUK TOMBOL ---

    private async void BtnRegistrasi_Click(object sender, RoutedEventArgs e)
        {
            string nama = txtNama.Text;
            string username = txtUsername.Text;
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            string telepon = txtTelepon.Text;
            string alamat = txtAlamat.Text;

            // Validasi sederhana
            if (string.IsNullOrWhiteSpace(nama) || 
                string.IsNullOrWhiteSpace(username) || 
                string.IsNullOrWhiteSpace(email) || 
                string.IsNullOrWhiteSpace(password) || 
                string.IsNullOrWhiteSpace(telepon) || 
                string.IsNullOrWhiteSpace(alamat))
            {
                MessageBox.Show("Semua field harus diisi!", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validasi email sederhana
            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Format email tidak valid!", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // DEMONSTRASI OOP: Menggunakan Customer class dengan ENCAPSULATION
                // ENCAPSULATION: Validation otomatis di property setter
                var newCustomer = new wpf.models.Customer();

                // Set data dan hash password via Register method
                newCustomer.Register(email, password, nama, alamat, telepon);

                try
                {
                    // Simpan ke database
                    var added = await _dbService.AddCustomerAsync(newCustomer);

                    MessageBox.Show($"Registrasi Customer berhasil!\n\n{added.GetUserInfo()}\n\nSilakan login dengan akun baru Anda.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Kembali ke LoginView (await the dispatcher invocation so we don't fire-and-forget)
                    await this.Dispatcher.InvokeAsync(() =>
                    {
                        // Resolve LoginView from DI container so dependencies are injected
                        var loginWindow = App.GetService<LoginView>();
                        loginWindow.Show();
                        this.Close();
                    }, System.Windows.Threading.DispatcherPriority.Background);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException dbex)
                {
                    // Kemungkinan duplicate email atau constraint violation
                    MessageBox.Show($"Registrasi gagal: {dbex.InnerException?.Message ?? dbex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Terjadi kesalahan saat registrasi: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (ArgumentException ex)
            {
                // ENCAPSULATION: Tangkap error dari validation
                MessageBox.Show($"Registrasi gagal: {ex.Message}", "Error Validasi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Terjadi kesalahan: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Kembali ke LoginView dengan Dispatcher (await to avoid fire-and-forget)
            await this.Dispatcher.InvokeAsync(() =>
            {
                var loginWindow = App.GetService<LoginView>();
                loginWindow.Show();
                this.Close();
            }, System.Windows.Threading.DispatcherPriority.Background);
        }

        private async void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            // Kembali ke LoginView dengan Dispatcher (await to avoid fire-and-forget)
            await this.Dispatcher.InvokeAsync(() =>
            {
                var loginWindow = App.GetService<LoginView>();
                loginWindow.Show();
                this.Close();
            }, System.Windows.Threading.DispatcherPriority.Background);
        }
        
        // --- METODE BANTUAN ---
        
        // Membuat window bisa di-drag
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Cek apakah yang diklik bukan input field
            if (e.Source == sender) 
            {
                this.DragMove();
            }
        }

        // --- Trik untuk Watermark PasswordBox ---
        
        // Saat PasswordBox mendapat fokus, sembunyikan watermark
        private void TxtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            var watermark = txtPassword.Template.FindName("Watermark", txtPassword) as TextBlock;
            if (watermark != null)
            {
                watermark.Visibility = Visibility.Collapsed;
            }
        }

        // Saat PasswordBox kehilangan fokus
        private void TxtPassword_LostFocus(object? sender, RoutedEventArgs? e)
        {
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
