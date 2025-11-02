using System.Windows;

namespace wpf.modules
{
    public partial class UpdateLocationDialog : Window
    {
        public string? LocationResult { get; private set; }

        public UpdateLocationDialog(string? currentLocation = null)
        {
            InitializeComponent();
            if (!string.IsNullOrWhiteSpace(currentLocation))
            {
                txtNewLocation.Text = currentLocation;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            LocationResult = txtNewLocation.Text?.Trim();
            this.DialogResult = true;
            this.Close();
        }
    }
}
