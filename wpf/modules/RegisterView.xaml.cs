using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace wpf.modules
{
    public partial class RegisterView : Window
    {
        public RegisterView()
        {
            InitializeComponent();
            
            // Tambahkan event handler agar window bisa di-drag
            this.MouseLeftButtonDown += OnMouseLeftButtonDown;
            
            // Trik untuk watermark PasswordBox
            txtPassword.GotFocus += TxtPassword_GotFocus;
            txtPassword.LostFocus += TxtPassword_LostFocus;
            TxtPassword_LostFocus(null, null);
        }

        // --- METODE UNTUK TOMBOL ---

        private void BtnRegistrasi_Click(object sender, RoutedEventArgs e)
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

            // Default role adalah Customer (karena radio button sudah dihapus)
            string role = "Customer";

            // Proses registrasi (implementasi sesuai kebutuhan)
            MessageBox.Show($"Registrasi {role} berhasil!\n\nNama: {nama}\nUsername: {username}\nEmail: {email}\n\nSilakan login dengan akun baru Anda.", 
                          "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
            
            // TODO: Simpan data ke database
            
            // Kembali ke LoginView
            LoginView loginWindow = new LoginView();
            loginWindow.Show();
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Kembali ke LoginView
            LoginView loginWindow = new LoginView();
            loginWindow.Show();
            this.Close();
        }

        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            // Kembali ke LoginView
            LoginView loginWindow = new LoginView();
            loginWindow.Show();
            this.Close();
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
