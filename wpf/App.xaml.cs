using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using Npgsql;
using wpf.Data;
using wpf.services;
using wpf.modules;

namespace wpf
{
    public partial class App : Application
    {
        private IHost? _host;

        public App()
        {
            // Build Generic Host untuk Dependency Injection
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Load appsettings.json dari working directory
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    // Make appsettings.json optional so the app can run using environment variables alone
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    // Ambil connection string dari configuration
                    var configuration = context.Configuration;
                    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;

                    // If connection string is provided in URI form (postgres://...), convert it to key/value format
                    if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) ||
                        connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            var uri = new Uri(connectionString);
                            var userInfo = uri.UserInfo.Split(':');
                            var username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : string.Empty;
                            var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;

                            var builder = new NpgsqlConnectionStringBuilder
                            {
                                Host = uri.Host,
                                Port = uri.Port,
                                Database = uri.AbsolutePath.TrimStart('/'),
                                Username = username,
                                Password = password,
                                SslMode = SslMode.Require
                            };

                            connectionString = builder.ConnectionString;
                        }
                        catch
                        {
                            // If parsing fails, keep original string and let Npgsql throw a descriptive error later
                        }
                    }

                    // Register DbContext dengan PostgreSQL
                    // Use DbContext pooling for better performance and set a reasonable command timeout.
                    services.AddDbContextPool<AppDbContext>(options =>
                        options.UseNpgsql(connectionString, npgsqlOptions => npgsqlOptions.CommandTimeout(60)));

                    // Register Services
                    services.AddScoped<PostgresService>();

                    // Register Views/Windows sebagai Transient
                    // Note: KapalDetailView tidak diregister karena memerlukan parameter constructor
                    // Instantiate langsung dengan 'new KapalDetailView(kapalData, parentView)'
                    services.AddTransient<LoginView>();
                    services.AddTransient<MainWindow>();
                    services.AddTransient<AdminView>();
                    services.AddTransient<CustomerView>();
                    services.AddTransient<RegisterView>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            // Start the host
            await _host!.StartAsync();

            // Resolve dan tampilkan LoginView sebagai window pertama
            var loginWindow = _host.Services.GetRequiredService<LoginView>();
            loginWindow.Show();

            // Set shutdown mode agar aplikasi tidak tertutup ketika LoginView ditutup
            // Aplikasi akan tetap berjalan selama ada window yang terbuka
            Application.Current.ShutdownMode = ShutdownMode.OnLastWindowClose;

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }
            base.OnExit(e);
        }

        /// <summary>
        /// Helper method untuk mendapatkan service dari DI container
        /// Berguna untuk diakses dari Views/ViewModels
        /// </summary>
        public static T GetService<T>() where T : class
        {
            var app = (App)Current;
            return app._host!.Services.GetRequiredService<T>();
        }
    }
}