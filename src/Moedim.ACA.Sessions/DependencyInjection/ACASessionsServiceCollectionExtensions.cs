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
        return services;
    }
}