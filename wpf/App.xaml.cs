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

            // Buat instance jendela Login dan tampilkan langsung
            // LoginView akan handle pembukaan window berikutnya (CustomerView atau MainWindow)
            LoginView loginWindow = new LoginView();
            loginWindow.Show();
            
            // Set shutdown mode agar aplikasi tidak tertutup ketika LoginView ditutup
            // Aplikasi akan tetap berjalan selama ada window yang terbuka
            Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;
        }
    }
}