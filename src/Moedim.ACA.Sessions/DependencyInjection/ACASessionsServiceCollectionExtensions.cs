using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moedim.ACA.Sessions;
using Moedim.ACA.Sessions.Options;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering ACA Sessions services for CodeInterpreter.
/// </summary>
public static class ACASessionsServiceCollectionExtensions
{
    /// <summary>
    /// Adds the ACA Sessions services to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddACASessions(
        this IServiceCollection services)
    {
        // Register any services required by the ACA Sessions library here.
        // For example, if there are any singleton or scoped services, they can be added like this:
        // services.AddSingleton<IMyService, MyServiceImplementation>();
        services.AddHttpClient();
        services.AddOptions<AzureTokenProviderOptions>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                var section = configuration.GetSection("ACASessions:AzureTokenProvider");

                // Assign properties explicitly to avoid Bind issues
                if (int.TryParse(section["RefreshBeforeMinutes"], out var refreshBeforeMinutes))
                {
                    settings.RefreshBeforeMinutes = refreshBeforeMinutes;
                }

                settings.Scopes = section["Scopes"]?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            });

        services.TryAddSingleton<IAzureTokenProvider>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureTokenProviderOptions>>();
            var logger = sp.GetRequiredService<ILogger<AzureTokenProvider>>();
            return new AzureTokenProvider(options, logger);
        });

        return services;
    }
}