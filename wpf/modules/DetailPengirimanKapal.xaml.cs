using System;
using System.Windows;
using System.Windows.Media;

namespace wpf.modules
{
    public partial class DetailPengirimanKapal : Window
    {
        private long _pengirimanId;

        public DetailPengirimanKapal(long pengirimanId)
        {
            InitializeComponent();
            _pengirimanId = pengirimanId;
            LoadDetailAsync();
        }

        private async void LoadDetailAsync()
        {
            try
            {
                var db = App.GetService<wpf.services.PostgresService>();
                var pengiriman = await db.GetPengirimanByIdAsync(_pengirimanId);

                if (pengiriman != null)
                {
                    // Display ID
                    txtIdBarang.Text = "#P" + pengiriman.PengirimanId.ToString();

                    // Nama Barang
                    txtNamaBarang.Text = pengiriman.NamaBarang ?? "-";

                    // Berat
                    txtBerat.Text = pengiriman.BeratKg.ToString("N2") + " Kg";

                    // Customer
                    try
                    {
                        if (pengiriman.CustomerId != 0)
                        {
                            var customer = await db.GetCustomerByIdAsync(pengiriman.CustomerId);
                            if (customer != null)
                            {
                                txtCustomer.Text = customer.Nama ?? "-";
                            }
                            else
                            {
                                txtCustomer.Text = "-";
                            }
                        }
                        else
                        {
                            txtCustomer.Text = "-";
                        }
                    }
                    catch
                    {
                        txtCustomer.Text = "-";
                    }

                    // Kapal
                    try
                    {
                        if (pengiriman.KapalId != 0)
                        {
                            var kapal = await db.GetKapalByIdAsync(pengiriman.KapalId);
                            if (kapal != null)
                            {
                                txtNamaKapal.Text = kapal.NamaKapal ?? "-";
                                txtLokasiTujuan.Text = kapal.LokasiTujuan ?? "-";
                                txtLokasiSekarang.Text = kapal.LokasiSekarang ?? "-";
                            }
                            else
                            {
                                txtNamaKapal.Text = "-";
                                txtLokasiTujuan.Text = "-";
                                txtLokasiSekarang.Text = "-";
                            }
                        }
                        else
                        {
                            txtNamaKapal.Text = "-";
                            txtLokasiTujuan.Text = "-";
                            txtLokasiSekarang.Text = "-";
                        }
                    }
                    catch
                    {
                        txtNamaKapal.Text = "-";
                        txtLokasiTujuan.Text = "-";
                        txtLokasiSekarang.Text = "-";
                    }

                    // Tanggal
                    txtTanggalMasuk.Text = pengiriman
                        .TanggalMulai.ToLocalTime()
                        .ToString("dd MMMM yyyy");
                    txtEstimasi.Text = pengiriman
                        .TanggalSelesaiEstimasi.ToLocalTime()
                        .ToString("dd MMMM yyyy");

                    // Status
                    string status = pengiriman.StatusPengiriman ?? "Pending";
                    txtStatus.Text = status;

                    // Set status color
                    if (status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    {
                        borderStatus.Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#90EE90")
                        );
                    }
                    else if (
                        status.Equals("Proses", StringComparison.OrdinalIgnoreCase)
                        || status.Equals("Diproses", StringComparison.OrdinalIgnoreCase)
                    )
                    {
                        borderStatus.Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#FFD700")
                        );
                    }
                    else if (status.Equals("Ditunda", StringComparison.OrdinalIgnoreCase))
                    {
                        borderStatus.Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#FFA07A")
                        );
                    }
                    else if (status.Equals("Selesai", StringComparison.OrdinalIgnoreCase))
                    {
                        borderStatus.Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#90EE90")
                        );
                    }
                    else
                    {
                        borderStatus.Background = new SolidColorBrush(
                            (Color)ColorConverter.ConvertFromString("#6C757D")
                        );
                    }

                    // Deskripsi
                    if (!string.IsNullOrEmpty(pengiriman.DeskripsiBarang))
                    {
                        var parts = pengiriman.DeskripsiBarang.Split('|');
                        if (parts.Length > 1)
                        {
                            txtDeskripsi.Text = parts[1];
                        }
                        else
                        {
                            txtDeskripsi.Text = pengiriman.DeskripsiBarang;
                        }
                    }
                    else
                    {
                        txtDeskripsi.Text = "-";
                    }

                    // Biaya
                    decimal biaya = pengiriman.CalculateCost();
                    txtBiaya.Text = "Rp " + biaya.ToString("N0");
                }
                else
                {
                    StyledMessageBox.ShowOk(this, "Error", "Data pengiriman tidak ditemukan.");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                StyledMessageBox.ShowOk(this, "Error", $"Gagal memuat detail: {ex.Message}");
                this.Close();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
