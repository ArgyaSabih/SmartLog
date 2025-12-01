using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace wpf.modules
{
    public partial class LocationDetailDialog : Window
    {
        private readonly string _location;
        private readonly string? _statusVerifikasi;

        public LocationDetailDialog(string location, string? statusVerifikasi = null)
        {
            InitializeComponent();
            _location = location ?? "-";
            _statusVerifikasi = statusVerifikasi;

            txtLocation.Text = _location;
            txtStatus.Text = _statusVerifikasi ?? "Unknown";
            txtStatusBig.Text = _statusVerifikasi ?? "Unknown";

            // Set status badge color
            SetStatusBadgeColor(_statusVerifikasi);

            this.Loaded += LocationDetailDialog_Loaded;
        }

        private void SetStatusBadgeColor(string? status)
        {
            var s = (status ?? "Pending").ToLowerInvariant();
            switch (s)
            {
                case "verified":
                    statusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1F4E0"));
                    txtStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F7A3A"));
                    break;
                case "rejected":
                    statusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F8D7DA"));
                    txtStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7A1A1A"));
                    break;
                default:
                    statusBadge.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF4C2"));
                    txtStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8A6D00"));
                    break;
            }
        }

        private async void LocationDetailDialog_Loaded(object? sender, RoutedEventArgs e)
        {
            await LoadWeatherAsync();
        }

        private async Task LoadWeatherAsync()
        {
            lblError.Visibility = Visibility.Collapsed;
            txtCondition.Text = "Memuat...";
            try
            {
                var kota = (_location ?? string.Empty).Split(',')[0].Trim();
                string? apiKey = Environment.GetEnvironmentVariable("API_KEY");
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    lblError.Text = "API_KEY belum diset pada environment. Silakan set API_KEY agar detail cuaca tampil.";
                    lblError.Visibility = Visibility.Visible;
                    txtCondition.Text = "Cuaca tidak tersedia";
                    return;
                }

                using var client = new HttpClient();
                var url = $"https://api.weatherapi.com/v1/current.json?key={Uri.EscapeDataString(apiKey)}&q={Uri.EscapeDataString(kota)}&aqi=no";
                var resp = await client.GetAsync(url);
                if (!resp.IsSuccessStatusCode)
                {
                    lblError.Text = $"Gagal mengambil data cuaca: {resp.StatusCode}";
                    lblError.Visibility = Visibility.Visible;
                    txtCondition.Text = "Cuaca tidak tersedia";
                    return;
                }

                using var stream = await resp.Content.ReadAsStreamAsync();
                using var doc = await JsonDocument.ParseAsync(stream);
                var root = doc.RootElement;
                var loc = root.GetProperty("location");
                var cur = root.GetProperty("current");

                var locName = loc.GetProperty("name").GetString() ?? kota;
                var localtime = loc.GetProperty("localtime").GetString() ?? string.Empty;
                var tempC = cur.GetProperty("temp_c").GetDecimal();
                var conditionText = cur.GetProperty("condition").GetProperty("text").GetString() ?? string.Empty;
                var iconUrl = cur.GetProperty("condition").GetProperty("icon").GetString() ?? string.Empty;
                var humidity = cur.GetProperty("humidity").GetInt32();
                var windKph = cur.GetProperty("wind_kph").GetDecimal();

                // Update UI on dispatcher
                this.Dispatcher.Invoke(() =>
                {
                    txtLocation.Text = string.IsNullOrWhiteSpace(locName) ? _location : locName;
                    txtLocalTime.Text = $"Waktu lokal: {localtime}";
                    txtTemp.Text = $"{tempC} °C";
                    txtCondition.Text = conditionText;
                    txtHumidity.Text = $"{humidity} %";
                    txtWind.Text = $"{windKph} kph";
                    txtStatusBig.Text = _statusVerifikasi ?? "Unknown";

                    // Load icon — WeatherAPI icon urls are relative like //cdn.weatherapi.com/.. or /..; ensure proper scheme
                    if (!string.IsNullOrWhiteSpace(iconUrl))
                    {
                        var fixedUrl = iconUrl.StartsWith("//") ? "https:" + iconUrl : (iconUrl.StartsWith("/") ? "https://api.weatherapi.com" + iconUrl : iconUrl);
                        try
                        {
                            imgCondition.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(fixedUrl));
                        }
                        catch { /* ignore icon load errors */ }
                    }
                });
            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lblError.Text = "Gagal memuat data cuaca: " + ex.Message;
                    lblError.Visibility = Visibility.Visible;
                    txtCondition.Text = "Cuaca tidak tersedia";
                });
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
