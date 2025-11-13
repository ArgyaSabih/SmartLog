using System;
using System.Windows;

namespace wpf.modules
{
    public partial class AddBarangDialog : Window
    {
        public string NamaBarang => txtNamaBarang.Text.Trim();
        public decimal BeratKg { get; private set; }
        public string Customer => txtCustomer.Text.Trim();

        public AddBarangDialog()
        {
            InitializeComponent();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NamaBarang))
            {
                MessageBox.Show("Nama barang tidak boleh kosong", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtBeratKg.Text.Trim(), out decimal berat) || berat <= 0)
            {
                MessageBox.Show("Berat harus berupa angka lebih dari 0 (satuan kg).", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Customer))
            {
                MessageBox.Show("Customer tidak boleh kosong", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            BeratKg = berat;
            this.DialogResult = true;
            this.Close();
        }
    }
}
