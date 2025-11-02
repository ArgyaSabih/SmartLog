using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using wpf.models;

namespace wpf.modules
{
    public partial class KapalCreateView : Window
    {
        private readonly AdminView _parent;

        public KapalCreateView(AdminView parent)
        {
            InitializeComponent();
            _parent = parent;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nama = txtNama.Text?.Trim() ?? string.Empty;
                string nomorStr = txtNomor.Text?.Trim() ?? string.Empty;
                string kapasitasStr = txtKapasitas.Text?.Trim() ?? string.Empty;
                string tujuan = txtTujuan.Text?.Trim() ?? string.Empty;
                string lokasiSekarang = txtLokasiSekarang.Text?.Trim() ?? string.Empty;
                string status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Pending";

                if (string.IsNullOrWhiteSpace(nama) || string.IsNullOrWhiteSpace(nomorStr) || string.IsNullOrWhiteSpace(kapasitasStr))
                {
                    MessageBox.Show("Mohon isi Nama Kapal, Nomor Registrasi, dan Kapasitas.", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(nomorStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int nomorRegistrasi))
                {
                    MessageBox.Show("Nomor Registrasi harus berupa angka (integer).", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(kapasitasStr, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal kapasitasTon))
                {
                    MessageBox.Show("Kapasitas harus berupa angka (contoh: 2500 atau 2500.5).", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var db = App.GetService<wpf.services.PostgresService>();

                var kapal = new Kapal
                {
                    NamaKapal = nama,
                    NomorRegistrasi = nomorRegistrasi,
                    KapasitasTon = kapasitasTon,
                    StatusVerifikasi = status
                };

                // Set tujuan (destination) separately from current location
                if (!string.IsNullOrWhiteSpace(tujuan)) kapal.LokasiTujuan = tujuan;

                // If initial current location provided, set it using UpdateLokasi (setter is private)
                if (!string.IsNullOrWhiteSpace(lokasiSekarang))
                {
                    kapal.UpdateLokasi(lokasiSekarang);
                }

                var added = await db.AddKapalAsync(kapal);

                // update parent UI: show tujuan and current location
                _parent.Kapals.Add(new KapalData
                {
                    NamaKapal = added.NamaKapal ?? string.Empty,
                    IdKapal = added.KapalId.ToString(),
                    NomorRegistrasi = added.NomorRegistrasi.ToString(),
                    Kapasitas = added.KapasitasTon.ToString("G", CultureInfo.InvariantCulture) + " Ton",
                    Tujuan = added.LokasiTujuan ?? string.Empty,
                    LokasiSekarang = added.LokasiSekarang ?? string.Empty
                });

                _parent.txtTotalKapal.Text = _parent.Kapals.Count.ToString();

                MessageBox.Show("Kapal berhasil ditambahkan.", "Sukses", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Gagal menambahkan kapal: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
