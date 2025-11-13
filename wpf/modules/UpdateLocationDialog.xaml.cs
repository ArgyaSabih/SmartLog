using System.Windows;

namespace wpf.modules
{
    public partial class UpdateLocationDialog : Window
    {
        public string? LocationResult { get; private set; }

        public UpdateLocationDialog(string? currentLocation = null)
        {
            InitializeComponent();
            // Populate locations from central constant and pre-select current location if present
            try
            {
                cmbLocations.ItemsSource = wpf.constants.LocationConstants.AllowedLocations;
                if (!string.IsNullOrWhiteSpace(currentLocation))
                {
                    var cur = currentLocation.Trim();
                    foreach (var item in wpf.constants.LocationConstants.AllowedLocations)
                    {
                        if (string.Equals(item, cur, StringComparison.OrdinalIgnoreCase))
                        {
                            cmbLocations.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            catch
            {
                // If anything goes wrong, leave ComboBox empty — user can still pick a value.
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Read selected value from ComboBox (items are strings from LocationConstants)
            if (cmbLocations.SelectedItem is string s)
            {
                LocationResult = s.Trim();
            }
            else
            {
                LocationResult = null;
            }
            this.DialogResult = true;
            this.Close();
        }
    }
}
