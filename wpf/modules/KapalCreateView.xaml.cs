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
                string kapasitasStr = txtKapasitas.Text?.Trim() ?? string.Empty;
                string tujuan = txtTujuan.Text?.Trim() ?? string.Empty;
                string lokasiSekarang = txtLokasiSekarang.Text?.Trim() ?? string.Empty;
                string status = (cmbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Pending";
                if (string.IsNullOrWhiteSpace(nama) || string.IsNullOrWhiteSpace(kapasitasStr))
                {
                    MessageBox.Show("Mohon isi Nama Kapal, Nomor Registrasi, dan Kapasitas.", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Generate REG-XXXXX (5 uppercase letters) and ensure uniqueness in DB (kode_registrasi column).
                var db = App.GetService<wpf.services.PostgresService>();

                // generator function for REG-XXXXX
                var rnd = new Random();
                string Generator()
                {
                    char[] arr = new char[5];
                    for (int i = 0; i < 5; i++) arr[i] = (char)('A' + rnd.Next(0, 26));
                    return "REG-" + new string(arr);
                }
                if (!decimal.TryParse(kapasitasStr, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal kapasitasTon))
                {
                    MessageBox.Show("Kapasitas harus berupa angka (contoh: 2500 atau 2500.5).", "Validasi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // db already obtained above for uniqueness check

                var kapal = new Kapal
                {
                    NamaKapal = nama,
                    // keep numeric column for compatibility, set to a timestamp-based int fallback
                    NomorRegistrasi = (int)(Math.Abs(DateTime.UtcNow.Ticks) % int.MaxValue),
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

                // Use AddKapalWithRetriesAsync which will retry on unique constraint violations
                var added = await db.AddKapalWithRetriesAsync(kapal, Generator, maxAttempts: 8);

                // update parent UI: show tujuan and current location
                _parent.Kapals.Add(new KapalData
                {
                    NamaKapal = added.NamaKapal ?? string.Empty,
                    IdKapal = added.KapalId.ToString(),
                    // display as KodeRegistrasi if present, otherwise fallback to numeric
                    NomorRegistrasi = added.KodeRegistrasi ?? added.NomorRegistrasi.ToString(),
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
                MessageBox.Show($"Gagal menambahkan kapal: {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
