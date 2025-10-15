using System.Linq;
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
    private static readonly char[] ScopeSeparators = new[] { ' ', ',', ';' };

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

                // Support multiple configuration shapes for arrays:
                // 1) Indexed keys (JSON arrays) -> ACASessions:AzureTokenProvider:Scopes:0, Scopes:1
                // 2) Single-string fallback with space/comma/semicolon separators -> ACASessions:AzureTokenProvider:Scopes
                var scopesSection = section.GetSection("Scopes");

                // Prefer a direct string value when present (e.g. "scope-a scope-b") then
                // fall back to child entries (e.g. Scopes:0, Scopes:1) which are used by
                // JSON arrays and indexed configuration providers.
                var scopesRawValue = scopesSection.Value;
                if (!string.IsNullOrEmpty(scopesRawValue))
                {
                    // Legacy/support for a single config key with separated values
                    settings.Scopes = scopesRawValue.Split(ScopeSeparators, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    // Read indexed/array-style configuration (e.g. Scopes:0, Scopes:1 or JSON array)
                    var childValues = scopesSection.GetChildren()
                        .Select(c => c.Value)
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Select(s => s!)
                        .ToArray();

                    settings.Scopes = childValues.Length > 0 ? childValues : Array.Empty<string>();
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
}