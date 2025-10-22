using Azure.Core;
using Moedim.ACA.Sessions.Impl;
using Moedim.ACA.Sessions.Options;

namespace Moedim.ACA.Sessions.UnitTest.Impl;

public class AzureTokenProviderTests
{
    private static readonly string[] DefaultScopes = ["https://dynamicsessions.io/.default"];

    [Fact(DisplayName = "GetTokenAsync with new scope acquires and caches token")]
    public async Task GetTokenAsync_WithNewScope_AcquiresAndCachesToken()
    {
        var tokenValue = "token-abc";
        var token = new AccessToken(tokenValue, DateTimeOffset.UtcNow.AddMinutes(10));

        var credential = new TestTokenCredential([token]);
        var logger = new NoOpLogger<AzureTokenProvider>();

        var options = new AzureTokenProviderOptions { RefreshBeforeMinutes = 1 };
        var ioptions = Microsoft.Extensions.Options.Options.Create(options);

        using var provider = new AzureTokenProvider(ioptions, logger, credential);

        var result = await provider.GetTokenAsync(DefaultScopes, CancellationToken.None);

        Assert.Equal(tokenValue, result.Token);
        Assert.Equal(token.ExpiresOn, result.ExpiresOn);
        Assert.Equal(1, credential.CallCount);
    }

    [Fact(DisplayName = "GetTokenAsync with cached scope returns cached token")]
    public async Task GetTokenAsync_WithCachedScope_ReturnsCachedToken()
    {
        var tokenValue = "cached-token";
        var token = new AccessToken(tokenValue, DateTimeOffset.UtcNow.AddMinutes(10));

        var credential = new TestTokenCredential([token]);
        var logger = new NoOpLogger<AzureTokenProvider>();

        var options = new AzureTokenProviderOptions { RefreshBeforeMinutes = 1 };
        var ioptions = Microsoft.Extensions.Options.Options.Create(options);

        using var provider = new AzureTokenProvider(ioptions, logger, credential);

        // First call acquires token
        var result1 = await provider.GetTokenAsync(DefaultScopes, CancellationToken.None);

        // Second call returns cached token
        var result2 = await provider.GetTokenAsync(DefaultScopes, CancellationToken.None);

        Assert.Equal(tokenValue, result1.Token);
        Assert.Equal(tokenValue, result2.Token);
        Assert.Equal(result1.ExpiresOn, result2.ExpiresOn);
        Assert.Equal(1, credential.CallCount); // Only one call to credential
    }

    [Fact(DisplayName = "GetTokenAsync with different scopes caches separately")]
    public async Task GetTokenAsync_WithDifferentScopes_CachesSeparately()
    {
        var token1Value = "token-scope1";
        var token2Value = "token-scope2";

        var token1 = new AccessToken(token1Value, DateTimeOffset.UtcNow.AddMinutes(10));
        var token2 = new AccessToken(token2Value, DateTimeOffset.UtcNow.AddMinutes(10));

        var credential = new TestTokenCredential([token1, token2]);
        var logger = new NoOpLogger<AzureTokenProvider>();

        var options = new AzureTokenProviderOptions { RefreshBeforeMinutes = 1 };
        var ioptions = Microsoft.Extensions.Options.Options.Create(options);

        using var provider = new AzureTokenProvider(ioptions, logger, credential);

        var scopes1 = new[] { "scope-a" };
        var scopes2 = new[] { "scope-b" };

        var result1 = await provider.GetTokenAsync(scopes1, CancellationToken.None);
        var result2 = await provider.GetTokenAsync(scopes2, CancellationToken.None);

        Assert.Equal(token1Value, result1.Token);
        Assert.Equal(token2Value, result2.Token);
        Assert.Equal(2, credential.CallCount); // Two calls for different scopes
    }

    [Fact(DisplayName = "Concurrent calls for same scope only trigger one acquisition")]
    public async Task GetTokenAsync_ConcurrentCallsSameScope_OnlyOneAcquisition()
    {
        var tokenValue = "concurrent-token";
        var token = new AccessToken(tokenValue, DateTimeOffset.UtcNow.AddMinutes(10));

        // Slow credential so concurrent callers overlap.
        var credential = new TestTokenCredential([token], delay: TimeSpan.FromMilliseconds(200));
        var logger = new NoOpLogger<AzureTokenProvider>();

        var options = new AzureTokenProviderOptions { RefreshBeforeMinutes = 1 };
        var ioptions = Microsoft.Extensions.Options.Options.Create(options);

        using var provider = new AzureTokenProvider(ioptions, logger, credential);

        var tasks = Enumerable.Range(0, 10).Select(_ => provider.GetTokenAsync(DefaultScopes, CancellationToken.None)).ToArray();

        var results = await Task.WhenAll(tasks);

        Assert.All(results, r => Assert.Equal(tokenValue, r.Token));
        Assert.Equal(1, credential.CallCount);
    }

    [Fact(DisplayName = "Expired token triggers refresh")]
    public async Task GetTokenAsync_ExpiredToken_TriggersRefresh()
    {
        var firstTokenValue = "first-token";
        var secondTokenValue = "second-token";

        // First token is already expired.
        var first = new AccessToken(firstTokenValue, DateTimeOffset.UtcNow.AddSeconds(-1));
        var second = new AccessToken(secondTokenValue, DateTimeOffset.UtcNow.AddMinutes(10));

        var credential = new TestTokenCredential([first, second]);
        var logger = new NoOpLogger<AzureTokenProvider>();

        // No refresh buffer so expired token forces refresh immediately on next call.
        var options = new AzureTokenProviderOptions { RefreshBeforeMinutes = 0 };
        var ioptions = Microsoft.Extensions.Options.Options.Create(options);

        using var provider = new AzureTokenProvider(ioptions, logger, credential);

        var t1 = await provider.GetTokenAsync(DefaultScopes, CancellationToken.None);
        Assert.Equal(firstTokenValue, t1.Token);

        var t2 = await provider.GetTokenAsync(DefaultScopes, CancellationToken.None);
        Assert.Equal(secondTokenValue, t2.Token);

        Assert.Equal(2, credential.CallCount);
    }

    [Fact(DisplayName = "GetTokenAsync sends requested scopes to credential")]
    public async Task GetTokenAsync_SendsRequestedScopes_ToCredential()
    {
        var scopes = new[] { "scope-a", "scope-b" };
        var options = new AzureTokenProviderOptions { RefreshBeforeMinutes = 1 };
        var ioptions = Microsoft.Extensions.Options.Options.Create(options);

        var tokenValue = "opt-token";
        var token = new AccessToken(tokenValue, DateTimeOffset.UtcNow.AddMinutes(10));

        var credential = new TestTokenCredential([token]);
        var logger = new NoOpLogger<AzureTokenProvider>();

        using var provider = new AzureTokenProvider(ioptions, logger, credential);

        var result = await provider.GetTokenAsync(scopes, CancellationToken.None);

        Assert.Equal(tokenValue, result.Token);
        Assert.NotNull(credential.LastRequestedScopes);
        Assert.Equal(scopes, credential.LastRequestedScopes);
    }

    [Fact(DisplayName = "GetTokenAsync with null scopes uses default scopes")]
    public async Task GetTokenAsync_WithNullScopes_UsesDefaultScopes()
    {
        var tokenValue = "default-scope-token";
        var token = new AccessToken(tokenValue, DateTimeOffset.UtcNow.AddMinutes(10));

        var credential = new TestTokenCredential([token]);
        var logger = new NoOpLogger<AzureTokenProvider>();

        var options = new AzureTokenProviderOptions { RefreshBeforeMinutes = 1 };
        var ioptions = Microsoft.Extensions.Options.Options.Create(options);

        using var provider = new AzureTokenProvider(ioptions, logger, credential);

        var result = await provider.GetTokenAsync(null!, CancellationToken.None);

        Assert.Equal(tokenValue, result.Token);
        Assert.NotNull(credential.LastRequestedScopes);
        Assert.Equal(DefaultScopes, credential.LastRequestedScopes);
    }

    [Fact(DisplayName = "ClearCache removes all cached tokens")]
    public async Task ClearCache_RemovesAllCachedTokens()
    {
        var token1Value = "token-1";
        var token2Value = "token-2";

        var token1 = new AccessToken(token1Value, DateTimeOffset.UtcNow.AddMinutes(10));
        var token2 = new AccessToken(token2Value, DateTimeOffset.UtcNow.AddMinutes(10));

        var credential = new TestTokenCredential([token1, token2]);
        var logger = new NoOpLogger<AzureTokenProvider>();

        var options = new AzureTokenProviderOptions { RefreshBeforeMinutes = 1 };
        var ioptions = Microsoft.Extensions.Options.Options.Create(options);

        using var provider = new AzureTokenProvider(ioptions, logger, credential);

        // Acquire token
        await provider.GetTokenAsync(DefaultScopes, CancellationToken.None);
        Assert.Equal(1, credential.CallCount);

        // Clear cache
        provider.ClearCache();

        // Should acquire new token
        await provider.GetTokenAsync(DefaultScopes, CancellationToken.None);
        Assert.Equal(2, credential.CallCount);
    }
}
