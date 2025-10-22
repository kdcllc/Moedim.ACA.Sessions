using System.Collections.Concurrent;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moedim.ACA.Sessions.Options;

namespace Moedim.ACA.Sessions.Impl;

/// <summary>
/// Enables Microsoft Entra Token provider to be used.
/// </summary>
internal class AzureTokenProvider : IAzureTokenProvider
{
    // LoggerMessage delegates to avoid allocation when logging is disabled.
    private static readonly Action<ILogger, string, Exception?> LogAcquiringToken = LoggerMessage.Define<string>(LogLevel.Debug, new EventId(1, nameof(GetTokenAsync)), "Acquiring new access token for scopes {Scopes}");
    private static readonly Action<ILogger, DateTimeOffset, Exception?> LogAcquiredToken = LoggerMessage.Define<DateTimeOffset>(LogLevel.Debug, new EventId(2, nameof(GetTokenAsync)), "Acquired access token expiring at {ExpiresOn}");

    // Default scopes used when none are provided.
    private static readonly string[] DefaultScopes = ["https://dynamicsessions.io/.default"];

    private readonly ILogger<AzureTokenProvider> _logger;

    // Credential used to acquire tokens. Created once and reused.
    private readonly TokenCredential _credential;

    // Refresh the token slightly before it actually expires to avoid edge cases.
    private readonly TimeSpan _refreshBefore;

    // Cache for tokens indexed by scope key. Each scope combination gets its own cache entry.
    private readonly ConcurrentDictionary<string, CachedToken> _tokenCache = new();

    // Per-scope semaphores to ensure only one refresh per scope at a time.
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _scopeSemaphores = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="AzureTokenProvider"/> class.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public AzureTokenProvider(
        IOptions<AzureTokenProviderOptions> options,
        ILogger<AzureTokenProvider> logger)
    {
        ArgumentNullException.ThrowIfNull(options);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _credential = new DefaultAzureCredential();
        _refreshBefore = TimeSpan.FromMinutes(options.Value.RefreshBeforeMinutes);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureTokenProvider"/> class using
    /// a custom <see cref="TokenCredential"/>. This constructor is intended for
    /// testing scenarios where a fake credential is provided.
    /// </summary>
    /// <param name="options">The options controlling refresh behavior.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="credential">The credential to use for acquiring tokens.</param>
    internal AzureTokenProvider(
        IOptions<AzureTokenProviderOptions> options,
        ILogger<AzureTokenProvider> logger,
        TokenCredential credential)
    {
        ArgumentNullException.ThrowIfNull(options);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _credential = credential ?? throw new ArgumentNullException(nameof(credential));
        _refreshBefore = TimeSpan.FromMinutes(options.Value.RefreshBeforeMinutes);
    }

    /// <inheritdoc/>
    public async Task<AccessToken> GetTokenAsync(string[] scopes, CancellationToken cancellationToken)
    {
        // Use default scopes if none provided
        var targetScopes = scopes?.Length > 0 ? scopes : DefaultScopes;

        // Create a cache key from the scopes (sorted for consistency)
        var scopeKey = CreateScopeKey(targetScopes);

        // Quick non-blocking check - if we have a cached token that's not near expiry, return it.
        if (_tokenCache.TryGetValue(scopeKey, out var cached) &&
            cached.IsAvailable &&
            cached.Token.ExpiresOn > DateTimeOffset.UtcNow.Add(_refreshBefore))
        {
            return cached.Token;
        }

        // Get or create a semaphore for this specific scope combination
        var semaphore = _scopeSemaphores.GetOrAdd(scopeKey, _ => new SemaphoreSlim(1, 1));

        // Only one caller should refresh the token for this scope at a time; others wait.
        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // Re-check after acquiring semaphore in case another caller already refreshed.
            if (_tokenCache.TryGetValue(scopeKey, out cached) &&
                cached.IsAvailable &&
                cached.Token.ExpiresOn > DateTimeOffset.UtcNow.Add(_refreshBefore))
            {
                return cached.Token;
            }

            var scopesForLog = targetScopes.Length == 1 ? targetScopes[0] : string.Join(' ', targetScopes);
            if (_logger != null)
            {
                LogAcquiringToken(_logger, scopesForLog, null);
            }

            var token = await _credential.GetTokenAsync(new TokenRequestContext(targetScopes), cancellationToken).ConfigureAwait(false);

            // Update or add the cached token
            var cachedToken = new CachedToken
            {
                Token = token,
                IsAvailable = true
            };
            _tokenCache[scopeKey] = cachedToken;

            if (_logger != null)
            {
                LogAcquiredToken(_logger, token.ExpiresOn, null);
            }

            return token;
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <inheritdoc/>
    public void ClearCache()
    {
        _tokenCache.Clear();
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes managed resources used by the provider.
    /// </summary>
    /// <param name="disposing">True when called from Dispose, false when called from finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var semaphore in _scopeSemaphores.Values)
            {
                semaphore?.Dispose();
            }

            _scopeSemaphores.Clear();
        }
    }

    /// <summary>
    /// Creates a consistent cache key from scopes by sorting them.
    /// </summary>
    private static string CreateScopeKey(string[] scopes)
    {
        var sortedScopes = scopes.OrderBy(s => s, StringComparer.Ordinal).ToArray();
        return string.Join("|", sortedScopes);
    }

    /// <summary>
    /// Represents a cached token with its access token and availability flag.
    /// </summary>
    private sealed class CachedToken
    {
        public AccessToken Token { get; set; }

        public bool IsAvailable { get; set; }
    }
}