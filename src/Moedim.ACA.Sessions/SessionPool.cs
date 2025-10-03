using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Moedim.ACA.Sessions;

/// <summary>
/// Default implementation of ISessionPool for managing Azure Container Apps sessions.
/// </summary>
public class SessionPool : ISessionPool, IDisposable
{
    private readonly SessionPoolOptions _options;
    private readonly ILogger<SessionPool> _logger;
    private readonly ConcurrentDictionary<string, SessionInfo> _sessions = new();
    private readonly SemaphoreSlim _semaphore;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionPool"/> class.
    /// </summary>
    /// <param name="options">Session pool options.</param>
    /// <param name="logger">Logger instance.</param>
    public SessionPool(IOptions<SessionPoolOptions> options, ILogger<SessionPool> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _semaphore = new SemaphoreSlim(_options.MaxConcurrentSessions, _options.MaxConcurrentSessions);
    }

    /// <inheritdoc/>
    public async Task<SessionInfo> GetOrCreateSessionAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            // Try to find an available session
            var availableSession = _sessions.Values.FirstOrDefault(s => 
                s.IsActive && s.ExpiresAt > DateTimeOffset.UtcNow);

            if (availableSession != null)
            {
                _logger.LogInformation("Reusing existing session {SessionId}", availableSession.Id);
                return availableSession;
            }

            // Create a new session
            var sessionId = Guid.NewGuid().ToString();
            var session = new SessionInfo
            {
                Id = sessionId,
                Endpoint = $"{_options.Endpoint}/sessions/{sessionId}",
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.SessionTimeoutMinutes),
                IsActive = true,
                Properties = new Dictionary<string, string>
                {
                    ["ContainerImage"] = _options.ContainerImage
                }
            };

            _sessions.TryAdd(sessionId, session);
            _logger.LogInformation("Created new session {SessionId}", sessionId);

            return session;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc/>
    public Task ReleaseSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            _logger.LogInformation("Released session {SessionId}", sessionId);
            // Session remains in pool for reuse
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task TerminateSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        if (_sessions.TryRemove(sessionId, out var session))
        {
            session.IsActive = false;
            _logger.LogInformation("Terminated session {SessionId}", sessionId);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<IEnumerable<SessionInfo>> GetActiveSessionsAsync(CancellationToken cancellationToken = default)
    {
        var activeSessions = _sessions.Values
            .Where(s => s.IsActive && s.ExpiresAt > DateTimeOffset.UtcNow)
            .ToList();

        return Task.FromResult<IEnumerable<SessionInfo>>(activeSessions);
    }

    /// <summary>
    /// Disposes the session pool.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _semaphore?.Dispose();
        _sessions.Clear();
        _disposed = true;

        GC.SuppressFinalize(this);
    }
}
