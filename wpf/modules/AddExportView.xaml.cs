using System;
using System.Linq;
using System.Windows;
using wpf.models;

namespace wpf.modules
{
    public partial class AddExportView : Window
    {
        private string _generatedCode = string.Empty;

        public AddExportView()
        {
            InitializeComponent();

            // Generate internal code (not shown in the form)
            _generatedCode = GenerateCode(5);

            txtDibuat.Text = DateTime.Now.ToString("dd MMM yyyy");
            txtEstimasi.Text = DateTime.Now.AddDays(4).ToString("dd MMM yyyy");

            // Load kapal list async safe (fire-and-forget is fine for small list)
            Loaded += async (_, __) =>
            {
                try
                {
                    var db = App.GetService<wpf.services.PostgresService>();
                    var kapals = await db.GetAllKapalsAsync();
                    cmbKapal.ItemsSource = kapals;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Gagal memuat daftar kapal: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
        }

        private string GenerateCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var rnd = new Random();
            return new string(Enumerable.Range(0, length).Select(_ => chars[rnd.Next(chars.Length)]).ToArray());
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Basic validation
            string nama = txtNamaBarang.Text?.Trim() ?? string.Empty;
            string beratText = txtBerat.Text?.Trim() ?? string.Empty;
            string detail = txtDetail.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(nama) || string.IsNullOrEmpty(beratText))
            {
                MessageBox.Show("Nama barang dan berat harus diisi.", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(beratText, out decimal beratKg) || beratKg <= 0)
            {
                MessageBox.Show("Berat harus berupa angka lebih dari 0.", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            long? customerId = Application.Current.Properties.Contains("CurrentUserId") ? Application.Current.Properties["CurrentUserId"] as long? : null;
            if (!customerId.HasValue)
            {
                MessageBox.Show("Tidak dapat menemukan session user. Silakan login ulang.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Determine selected kapal (optional)
            long kapalId = 0;
            if (cmbKapal.SelectedItem is Kapal k) kapalId = k.KapalId;

            // Build Pengiriman object
            var pengiriman = new Pengiriman();
            pengiriman.NamaBarang = nama;
            pengiriman.BeratKg = beratKg;
            pengiriman.CustomerId = customerId.Value;
            pengiriman.KapalId = kapalId;
            // Use UTC when persisting to Postgres timestamptz
            pengiriman.TanggalMulai = DateTime.UtcNow;
            pengiriman.TanggalSelesaiEstimasi = DateTime.UtcNow.AddDays(4);
            // Set initial status to Pending
            pengiriman.UpdateStatus("Pending");

            // Store generated code (without leading '#') in DeskripsiBarang so UI can display it as ID
            pengiriman.DeskripsiBarang = _generatedCode;
            if (!string.IsNullOrEmpty(detail))
            {
                pengiriman.DeskripsiBarang += "|" + detail;
            }
            

            try
            {
                var db = App.GetService<wpf.services.PostgresService>();
                var saved = await db.AddPengirimanAsync(pengiriman);

                MessageBox.Show($"Pengiriman berhasil ditambahkan (ID: #{_generatedCode}).", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal menyimpan pengiriman: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show(ex.ToString());
                Console.WriteLine(ex);
                Console.WriteLine(MessageBoxImage.Error);
            }
        }
    }
}
