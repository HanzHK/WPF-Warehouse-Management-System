using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace App.Presentation.Wpf.AppHost;

/// <summary>
/// Responsible for creating and configuring the application Host.
/// This is the composition root of the WPF application.
/// </summary>
public static class AppHostBuilder
{
    /// <summary>
    /// Builds and configures the IHost instance used by the WPF application.
    /// </summary>
    public static IHost CreateHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                // Base directory for configuration files
                config.SetBasePath(AppContext.BaseDirectory);

                // Optional appsettings.json (if added later)
                config.AddJsonFile("appsettings.json", optional: true);

                // Load User Secrets (Azure credentials, DB connection string, etc.)
                config.AddUserSecrets<App>();
            })
            .ConfigureServices((context, services) =>
            {
                IConfiguration configuration = context.Configuration;

                // DI registration will be added step-by-step later.
                // services.AddApplication();
                // services.AddInfrastructure(configuration["Database:ConnectionString"]);
                // services.AddSingleton<MainWindow>();
            })
            .Build();
    }
}
