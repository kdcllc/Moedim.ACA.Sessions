using System.Collections.Concurrent;
using Azure.Core;
using Microsoft.Extensions.Logging;
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

        using var provider = new AzureTokenProvider(logger, credential, TimeSpan.FromMinutes(1));

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

        using var provider = new AzureTokenProvider(logger, credential, TimeSpan.FromMinutes(1));

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
        using var provider = new AzureTokenProvider(logger, credential, TimeSpan.Zero);

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
        var options = new AzureTokenProviderOptions { Scopes = scopes };
        var ioptions = Microsoft.Extensions.Options.Options.Create(options);

        var tokenValue = "opt-token";
        var token = new AccessToken(tokenValue, DateTimeOffset.UtcNow.AddMinutes(10));

        var credential = new TestTokenCredential([token]);
        var logger = new NoOpLogger<AzureTokenProvider>();

        using var provider = new AzureTokenProvider(logger, ioptions, credential, TimeSpan.FromMinutes(1));

        var result = await provider.GetTokenAsync(CancellationToken.None);

        Assert.Equal(tokenValue, result);
        Assert.NotNull(credential.LastRequestedScopes);
        Assert.Equal(scopes, credential.LastRequestedScopes);
    }

    // Simple test TokenCredential that returns configured AccessToken instances in order.
    private class TestTokenCredential : TokenCredential
    {
        private readonly ConcurrentQueue<AccessToken> _tokens;
        private readonly TimeSpan _delay;
        private int _callCount;

        public TestTokenCredential(IEnumerable<AccessToken> tokens, TimeSpan? delay = null)
        {
            _tokens = new ConcurrentQueue<AccessToken>(tokens);
            _delay = delay ?? TimeSpan.Zero;
        }

        public int CallCount => Volatile.Read(ref _callCount);

        public string[] LastRequestedScopes { get; private set; } = Array.Empty<string>();

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _callCount);
            LastRequestedScopes = requestContext.Scopes.ToArray();
            if (_delay > TimeSpan.Zero)
            {
                Task.Delay(_delay, cancellationToken).GetAwaiter().GetResult();
            }

            if (_tokens.TryDequeue(out var token))
            {
                return token;
            }

            return new AccessToken("default", DateTimeOffset.UtcNow.AddMinutes(5));
        }

        public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _callCount);
            LastRequestedScopes = requestContext.Scopes.ToArray();

            if (_delay > TimeSpan.Zero)
            {
                await Task.Delay(_delay, cancellationToken).ConfigureAwait(false);
            }

            if (_tokens.TryDequeue(out var token))
            {
                return token;
            }

            return new AccessToken("default", DateTimeOffset.UtcNow.AddMinutes(5));
        }
    }

    // Minimal no-op logger implementation to avoid bringing logging packages into the tests.
    private sealed class NoOpLogger<T> : ILogger<T>
    {
        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
        }

        private sealed class NoopDisposable : IDisposable
        {
            public static readonly NoopDisposable Instance = new NoopDisposable();

            public void Dispose()
            {
            }
        }
    }
}
