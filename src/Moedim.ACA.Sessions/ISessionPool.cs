namespace Moedim.ACA.Sessions;

/// <summary>
/// Interface for managing session pools in Azure Container Apps.
/// </summary>
public interface ISessionPool
{
    /// <summary>
    /// Gets or creates a session.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A session info object.</returns>
    Task<SessionInfo> GetOrCreateSessionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Releases a session back to the pool.
    /// </summary>
    /// <param name="sessionId">The session ID to release.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ReleaseSessionAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminates a session.
    /// </summary>
    /// <param name="sessionId">The session ID to terminate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task TerminateSessionAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active sessions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of active sessions.</returns>
    Task<IEnumerable<SessionInfo>> GetActiveSessionsAsync(CancellationToken cancellationToken = default);
}
