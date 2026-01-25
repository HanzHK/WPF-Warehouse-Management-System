using Microsoft.Extensions.DependencyInjection;

namespace WMS.Application;

/// <summary>
/// Registers Application layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Application layer services to the DI container.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Application services will be registered here later.
        return services;
    }
}
