using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moedim.ACA.Sessions.Options;

namespace Moedim.ACA.Sessions.UnitTest.DependencyInjection;

public class ACASessionsServiceCollectionExtensionsTests
{
    [Fact]
    public void AddACASessions_WhenCalled_RegistersHttpClientFactory()
    {
        var services = new ServiceCollection();

        // Provide an IConfiguration so the extension's Configure<IConfiguration> can be resolved.
        var config = new ConfigurationBuilder().AddInMemoryCollection([]).Build();
        services.AddSingleton<IConfiguration>(config);

        services.AddACASessions();

        using var provider = services.BuildServiceProvider();

        var factory = provider.GetService<IHttpClientFactory>();

        Assert.NotNull(factory);
    }

    [Fact]
    public void AddACASessions_WithConfiguration_ConfiguresAzureTokenProviderOptions()
    {
        var services = new ServiceCollection();

        var values = new Dictionary<string, string?>
        {
            ["ACASessions:AzureTokenProvider:RefreshBeforeMinutes"] = "15"
        };

        var config = new ConfigurationBuilder().AddInMemoryCollection(values).Build();
        services.AddSingleton<IConfiguration>(config);

        services.AddACASessions();

        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<AzureTokenProviderOptions>>().Value;

        Assert.Equal(15, options.RefreshBeforeMinutes);
    }

    [Fact]
    public void AddACASessions_WithMissingConfiguration_UsesDefaults()
    {
        var services = new ServiceCollection();

        var config = new ConfigurationBuilder().AddInMemoryCollection([]).Build();
        services.AddSingleton<IConfiguration>(config);

        services.AddACASessions();

        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<AzureTokenProviderOptions>>().Value;

        Assert.Equal(0, options.RefreshBeforeMinutes);
    }
}
