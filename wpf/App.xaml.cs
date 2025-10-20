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

            // Buat instance jendela Login terlebih dahulu
            // User bisa memilih untuk Login atau pindah ke Register
            LoginView loginWindow = new LoginView();

            // Tampilkan jendela Login sebagai dialog (modal)
            bool? loginResult = loginWindow.ShowDialog();

            // Jika login berhasil, buka MainWindow
            if (loginResult == true)
            {
                // Jika login berhasil, buat dan tampilkan MainWindow
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            else
            {
                // Jika login dibatalkan atau ditutup, matikan aplikasi
                Application.Current.Shutdown();
            }
        }
    }
}