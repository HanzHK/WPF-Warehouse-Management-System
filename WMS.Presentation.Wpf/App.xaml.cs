using Microsoft.Extensions.Hosting;
using System.Windows;
using App.Presentation.Wpf.AppHost;

namespace App.Presentation.Wpf;

/// <summary>
/// WPF application entry point.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Global application host containing DI container and configuration.
    /// </summary>
    public static IHost AppHost { get; private set; }

    public App()
    {
        AppHost = AppHostBuilder.CreateHost();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost.StartAsync();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
        base.OnExit(e);
    }
}
