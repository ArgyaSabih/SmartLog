using System.Configuration; // Tambahkan using ini
using Supabase;

namespace wpf.Services
{
    public static class SupabaseService
    {
        // Ambil nilai dari app.config
        private static readonly string SupabaseUrl = ConfigurationManager.AppSettings["SupabaseUrl"] ?? "";
        private static readonly string SupabaseAnonKey = ConfigurationManager.AppSettings["SupabaseAnonKey"] ?? "";
        public static Client? Client { get; private set; }

        public static async Task InitializeAsync()
        {
            // Pastikan nilai tidak kosong sebelum digunakan
            if (string.IsNullOrEmpty(SupabaseUrl) || string.IsNullOrEmpty(SupabaseAnonKey))
            {
                throw new InvalidOperationException("Supabase URL and Key must be set in app.config");
            }

            Client = new Client(SupabaseUrl, SupabaseAnonKey);
            await Client.InitializeAsync();
        }
    }
}