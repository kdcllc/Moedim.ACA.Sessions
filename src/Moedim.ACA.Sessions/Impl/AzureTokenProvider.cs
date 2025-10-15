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

    // Default scopes used when none are provided by options.
    private static readonly string[] DefaultScopes = ["https://dynamicsessions.io/.default"];

    private readonly ILogger<AzureTokenProvider> _logger;

    // Credential used to acquire tokens. Created once and reused.
    private readonly TokenCredential _credential;

    // Semaphore to ensure only one refresh at a time.
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    // Refresh the token slightly before it actually expires to avoid edge cases.
    private readonly TimeSpan _refreshBefore;

    // Scopes this provider will request tokens for.
    private readonly string[] _scopes;

    // Cached token and flag indicating availability.
    private AccessToken _cachedToken;
    private volatile bool _hasCachedToken;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AzureTokenProvider"/> class.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public AzureTokenProvider(
        IOptions<AzureTokenProviderOptions> options,
        ILogger<AzureTokenProvider> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _credential = new DefaultAzureCredential();
        _refreshBefore = TimeSpan.FromMinutes(options.Value.RefreshBeforeMinutes);
        ArgumentNullException.ThrowIfNull(options);

        _scopes = options.Value.Scopes?.ToArray() ?? DefaultScopes;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureTokenProvider"/> class using
    /// a custom <see cref="TokenCredential"/>. This constructor is intended for
    /// testing scenarios where a fake credential is provided.
    /// </summary>
    /// <param name="options">The options controlling refresh behavior and scopes.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="credential">The credential to use for acquiring tokens.</param>
    internal AzureTokenProvider(
        IOptions<AzureTokenProviderOptions> options,
        ILogger<AzureTokenProvider> logger,
        TokenCredential credential)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _credential = credential ?? throw new ArgumentNullException(nameof(credential));
        _refreshBefore = TimeSpan.FromMinutes(options.Value.RefreshBeforeMinutes);
        ArgumentNullException.ThrowIfNull(options);

        _scopes = options.Value.Scopes?.ToArray() ?? DefaultScopes;
    }

    /// <inheritdoc/>
    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        // Quick non-blocking check - if we have a cached token that's not near expiry, return it.
        if (_hasCachedToken && _cachedToken.ExpiresOn > DateTimeOffset.UtcNow.Add(_refreshBefore))
        {
            return _cachedToken.Token;
        }

        // Only one caller should refresh the token at a time; others wait.
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // Re-check after acquiring semaphore in case another caller already refreshed.
            if (_hasCachedToken && _cachedToken.ExpiresOn > DateTimeOffset.UtcNow.Add(_refreshBefore))
            {
                return _cachedToken.Token;
            }

            var scopesForLog = _scopes.Length == 1 ? _scopes[0] : string.Join(' ', _scopes);
            if (_logger != null)
            {
                LogAcquiringToken(_logger, scopesForLog, null);
            }

            var token = await _credential.GetTokenAsync(new TokenRequestContext(_scopes), cancellationToken).ConfigureAwait(false);

            _cachedToken = token;
            _hasCachedToken = true;

            if (_logger != null)
            {
                LogAcquiredToken(_logger, token.ExpiresOn, null);
            }

            return token.Token;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc/>
    public void ClearCache()
    {
        _hasCachedToken = false;
        _cachedToken = default;
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
            _semaphore?.Dispose();
        }
    }
}