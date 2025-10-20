using System.Windows;

using wpf.Services;
namespace wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        await SupabaseService.InitializeAsync();
    }
}

