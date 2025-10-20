using System.Windows;
// 1. Tambahkan using untuk folder Modules Anda
using wpf.modules; // Ganti "WpfLoginApp" dengan nama namespace proyek Anda

namespace wpf // Ganti dengan nama namespace proyek Anda
{
    public partial class App : Application
    {
        // 2. Tambahkan method override OnStartup
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Buat instance jendela Login
            LoginView loginWindow = new LoginView();

            // Tampilkan jendela Login sebagai dialog (modal)
            // Kode akan berhenti di sini sampai jendela login ditutup
            bool? dialogResult = loginWindow.ShowDialog();

            // Periksa hasil dari dialog. 
            // Kita akan atur agar Login.xaml mengembalikan 'true' jika berhasil.
            if (dialogResult == true)
            {
                // Jika login berhasil, buat dan tampilkan MainWindow
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                // Jika login gagal, dibatalkan, atau ditutup [x],
                // matikan aplikasi.
                Application.Current.Shutdown();
            }
        }
    }
}