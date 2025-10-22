using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moedim.ACA.Sessions;
using Moedim.ACA.Sessions.Impl;
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
        services.AddCodeInterpreter();
        services.AddSessionsHttpClient();
        services.AddAzureTokenProvider();
        return services;
    }

    /// <summary>
    /// Adds the CodeInterpreter service to the specified IServiceCollection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCodeInterpreter(this IServiceCollection services)
    {
        services.TryAddScoped<ICodeInterpreter, CodeInterpreter>();
        return services;
    }

    /// <summary>
    /// Adds the AzureTokenProvider service to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the service to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddAzureTokenProvider(this IServiceCollection services)
    {
        services.AddOptions<AzureTokenProviderOptions>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                var section = configuration.GetSection("ACASessions:AzureTokenProvider");

                // Assign properties explicitly to avoid Bind issues
                if (int.TryParse(section["RefreshBeforeMinutes"], out var refreshBeforeMinutes))
                {
                    settings.RefreshBeforeMinutes = refreshBeforeMinutes;
                }
            });

        services.TryAddSingleton<IAzureTokenProvider>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureTokenProviderOptions>>();
            var logger = sp.GetRequiredService<ILogger<AzureTokenProvider>>();
            return new AzureTokenProvider(options, logger);
        });

        return services;
    }

    /// <summary>
    /// Adds the SessionsHttpClient service to the specified IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the service to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddSessionsHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<ISessionsHttpClient, SessionsHttpClient>();

        services.AddOptions<SessionsHttpClientOptions>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                var section = configuration.GetSection("ACASessions:SessionsHttpClient");

                // Assign properties explicitly to avoid Bind issues
                if (Uri.TryCreate(section["Endpoint"], UriKind.Absolute, out var baseUrl))
                {
                    settings.Endpoint = baseUrl;
                }

                if (section["ApiVersion"] is string apiVersion && !string.IsNullOrEmpty(apiVersion))
                {
                    settings.ApiVersion = apiVersion;
                }
            });

        return services;
    }
}