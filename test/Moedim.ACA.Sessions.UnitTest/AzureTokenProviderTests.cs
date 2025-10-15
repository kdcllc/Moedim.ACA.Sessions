using Azure.Core;
using Moedim.ACA.Sessions.Options;

namespace Moedim.ACA.Sessions.UnitTest;

public class AzureTokenProviderTests
{
    [Fact]
    public async Task GetTokenAsync_ReturnsTokenFromCredential()
    {
        var tokenValue = "token-abc";
        var token = new AccessToken(tokenValue, DateTimeOffset.UtcNow.AddMinutes(10));

        var credential = new TestTokenCredential([token]);
        var logger = new NoOpLogger<AzureTokenProvider>();

        var options = new AzureTokenProviderOptions { Scopes = ["https://dynamicsessions.io/.default"], RefreshBeforeMinutes = 1 };
        var ioptions = Microsoft.Extensions.Options.Options.Create(options);

        using var provider = new AzureTokenProvider(ioptions, logger, credential);

        var result = await provider.GetTokenAsync(CancellationToken.None);

        Assert.Equal(tokenValue, result);
        Assert.Equal(1, credential.CallCount);
    }

    [Fact]
    public async Task ConcurrentCalls_OnlyOneRequestToCredential()
    {
        var tokenValue = "concurrent-token";
        var token = new AccessToken(tokenValue, DateTimeOffset.UtcNow.AddMinutes(10));

        // Slow credential so concurrent callers overlap.
        var credential = new TestTokenCredential([token], delay: TimeSpan.FromMilliseconds(200));
        var logger = new NoOpLogger<AzureTokenProvider>();

        var options = new AzureTokenProviderOptions { Scopes = ["https://dynamicsessions.io/.default"], RefreshBeforeMinutes = 1 };
        var ioptions = Microsoft.Extensions.Options.Options.Create(options);

        using var provider = new AzureTokenProvider(ioptions, logger, credential);

        var tasks = Enumerable.Range(0, 10).Select(_ => provider.GetTokenAsync(CancellationToken.None)).ToArray();

        var results = await Task.WhenAll(tasks);

        Assert.All(results, r => Assert.Equal(tokenValue, r));
        Assert.Equal(1, credential.CallCount);
    }

    [Fact]
    public async Task ExpiredToken_TriggersRefresh()
    {
        var firstTokenValue = "first-token";
        var secondTokenValue = "second-token";

        // First token is already expired.
        var first = new AccessToken(firstTokenValue, DateTimeOffset.UtcNow.AddSeconds(-1));
        var second = new AccessToken(secondTokenValue, DateTimeOffset.UtcNow.AddMinutes(10));

        var credential = new TestTokenCredential([first, second]);
        var logger = new NoOpLogger<AzureTokenProvider>();

        // No refresh buffer so expired token forces refresh immediately on next call.
        var options = new AzureTokenProviderOptions { Scopes = ["https://dynamicsessions.io/.default"], RefreshBeforeMinutes = 0 };
        var ioptions = Microsoft.Extensions.Options.Options.Create(options);

        using var provider = new AzureTokenProvider(ioptions, logger, credential);

        var t1 = await provider.GetTokenAsync(CancellationToken.None);
        Assert.Equal(firstTokenValue, t1);

        var t2 = await provider.GetTokenAsync(CancellationToken.None);
        Assert.Equal(secondTokenValue, t2);

        Assert.Equal(2, credential.CallCount);
    }

    [Fact]
    public async Task UsesOptions_SendsScopesFromOptions()
    {
        var scopes = new[] { "scope-a", "scope-b" };
        var options = new AzureTokenProviderOptions { Scopes = scopes, RefreshBeforeMinutes = 1 };
        var ioptions = Microsoft.Extensions.Options.Options.Create(options);

        var tokenValue = "opt-token";
        var token = new AccessToken(tokenValue, DateTimeOffset.UtcNow.AddMinutes(10));

        var credential = new TestTokenCredential([token]);
        var logger = new NoOpLogger<AzureTokenProvider>();

        using var provider = new AzureTokenProvider(ioptions, logger, credential);

        var result = await provider.GetTokenAsync(CancellationToken.None);

        Assert.Equal(tokenValue, result);
        Assert.NotNull(credential.LastRequestedScopes);
        Assert.Equal(scopes, credential.LastRequestedScopes);
    }
}
