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
            string username = txtEmail.Text;
            string password = txtPassword.Password;

            if (username == "admin" && password == "password123")
            {
                MessageBox.Show("Login Berhasil!");
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Email atau password salah.");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        
        // --- METODE BANTUAN ---

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fungsi 'Create Account' belum diimplementasikan.");
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