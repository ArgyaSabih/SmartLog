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
            // Attempt to load .env from the wpf project folder (or nearby locations).
            // This sets environment variables like API_KEY so the WPF app can read them at runtime.
            try
            {
                var found = false;

                var candidates = new[]
                {
                    Path.Combine(Directory.GetCurrentDirectory(), ".env"),
                    Path.Combine(Directory.GetCurrentDirectory(), "wpf", ".env"),
                    Path.Combine(AppContext.BaseDirectory, ".env"),
                    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".env"),
                    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "wpf", ".env"),
                };

                foreach (var candidate in candidates)
                {
                    try
                    {
                        var full = Path.GetFullPath(candidate);
                        if (!File.Exists(full))
                            continue;

                        var lines = File.ReadAllLines(full);
                        foreach (var raw in lines)
                        {
                            var line = raw?.Trim();
                            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                                continue;
                            var idx = line.IndexOf('=');
                            if (idx <= 0) continue;
                            var k = line.Substring(0, idx).Trim();
                            var v = line.Substring(idx + 1).Trim();
                            if ((v.StartsWith("\"") && v.EndsWith("\"")) || (v.StartsWith("'") && v.EndsWith("'")))
                            {
                                v = v.Substring(1, v.Length - 2);
                            }
                            Environment.SetEnvironmentVariable(k, v);
                        }

                        // Stop after the first existing .env
                        found = true;
                        break;
                    }
                    catch
                    {
                        // ignore parsing errors and try next candidate
                    }
                }

                // If not found on disk, attempt reading an embedded .env resource inside the assembly
                if (!found)
                {
                    try
                    {
                        var asm = typeof(App).Assembly;
                        var names = asm.GetManifestResourceNames();
                        var envRes = names.FirstOrDefault(n => n.EndsWith(".env", StringComparison.OrdinalIgnoreCase));
                        if (envRes != null)
                        {
                            using var s = asm.GetManifestResourceStream(envRes);
                            if (s != null)
                            {
                                using var reader = new System.IO.StreamReader(s);
                                while (!reader.EndOfStream)
                                {
                                    var raw = reader.ReadLine();
                                    var line = raw?.Trim();
                                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                                        continue;
                                    var idx = line.IndexOf('=');
                                    if (idx <= 0) continue;
                                    var k = line.Substring(0, idx).Trim();
                                    var v = line.Substring(idx + 1).Trim();
                                    if ((v.StartsWith("\"") && v.EndsWith("\"")) || (v.StartsWith("'") && v.EndsWith("'")))
                                    {
                                        v = v.Substring(1, v.Length - 2);
                                    }
                                    Environment.SetEnvironmentVariable(k, v);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // ignore; best-effort only
                    }
                }
            }
            catch
            {
                // best-effort only; don't fail startup if .env not present or unreadable
            }
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