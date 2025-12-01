using System.Windows;

namespace wpf.modules
{
    public partial class StyledMessageBox : Window
    {
        public StyledMessageBox(string title, string message, bool showNo)
        {
            InitializeComponent();
            txtTitle.Text = title;
            txtMessage.Text = message;
            btnNo.Visibility = showNo ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        public static bool ShowOk(Window owner, string title, string message)
        {
            var dlg = new StyledMessageBox(title, message, false);
            dlg.Owner = owner;
            var res = dlg.ShowDialog();
            return res == true;
        }

        public static bool ShowConfirm(Window owner, string title, string message)
        {
            var dlg = new StyledMessageBox(title, message, true);
            dlg.Owner = owner;
            var res = dlg.ShowDialog();
            return res == true;
        }
    }
}
