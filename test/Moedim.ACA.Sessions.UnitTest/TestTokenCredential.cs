using System.Collections.Concurrent;
using Azure.Core;

namespace Moedim.ACA.Sessions.UnitTest;

// Simple test TokenCredential that returns configured AccessToken instances in order.
internal sealed class TestTokenCredential : TokenCredential
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