using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Moedim.ACA.Sessions;

/// <summary>
/// Extension methods for registering Moedim.ACA.Sessions services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the session pool and code interpreter plugin to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configuration action for session pool options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddACASessionPool(
        this IServiceCollection services,
        Action<SessionPoolOptions>? configure = null)
    {
        if (configure != null)
        {
            services.Configure(configure);
        }

        services.TryAddSingleton<ISessionPool, SessionPool>();
        services.TryAddTransient<PythonCodeInterpreterPlugin>();

        return services;
    }
}
