using Moedim.ACA.Sessions.Agents;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for registering ACA Sessions services for AI Agents.
/// </summary>
public static class AIAgentsACASessionsServiceCollectionExtensions
{
    /// <summary>
    /// Adds the ACA Sessions services and AI Agent plugins to the specified IServiceCollection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAIAgentsACASessions(
        this IServiceCollection services)
    {
        services.AddACASessions();

        services.AddScoped<CodeInterpreterPlugin>();

        return services;
    }
}