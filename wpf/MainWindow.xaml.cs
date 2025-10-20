// Pastikan namespace ini SAMA PERSIS dengan bagian pertama x:Class
using System.Windows;
namespace wpf
{
    // Pastikan nama class ini SAMA PERSIS dengan bagian kedua x:Class
    // dan pastikan ada kata 'partial'
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); // Error Anda ada di sini
        }
    }
}