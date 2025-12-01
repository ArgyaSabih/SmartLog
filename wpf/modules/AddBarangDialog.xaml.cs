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
                StyledMessageBox.ShowOk(this, "Validasi", "Nama barang tidak boleh kosong");
                return;
            }

            if (!decimal.TryParse(txtBeratKg.Text.Trim(), out decimal berat) || berat <= 0)
            {
                StyledMessageBox.ShowOk(this, "Validasi", "Berat harus berupa angka lebih dari 0 (satuan kg).");
                return;
            }

            if (string.IsNullOrWhiteSpace(Customer))
            {
                StyledMessageBox.ShowOk(this, "Validasi", "Customer tidak boleh kosong");
                return;
            }

            BeratKg = berat;
            this.DialogResult = true;
            this.Close();
        }
    }
}
