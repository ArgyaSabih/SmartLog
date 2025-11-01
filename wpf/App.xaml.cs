using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
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
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    // Ambil connection string dari configuration
                    var configuration = context.Configuration;
                    var connectionString = configuration.GetConnectionString("DefaultConnection");

                    // Register DbContext dengan PostgreSQL
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(connectionString));

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