using Moedim.ACA.Sessions.Example;

namespace Microsoft.Extensions.DependencyInjection;

internal static class ConsoleServiceCollectionExtensions
{
    public static void ConfigureServices(HostBuilderContext hostBuilder, IServiceCollection services)
    {
        services.AddScoped<IMain, Main>();

        // Register ACA Sessions services
        services.AddAIAgentsACASessions();

        services.AddAgent();
    }

    public static IServiceCollection AddAgent(this IServiceCollection services)
    {
        // Register the Code Interpreter service
        services.AddScoped<CodeAgent>();
        return services;
    }
}
