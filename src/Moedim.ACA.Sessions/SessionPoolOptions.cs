namespace Moedim.ACA.Sessions;

/// <summary>
/// Configuration options for the session pool.
/// </summary>
public class SessionPoolOptions
{
    /// <summary>
    /// Gets or sets the endpoint URL for Azure Container Apps Sessions.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Gets or sets the API key for authentication.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of concurrent sessions.
    /// Default is 10.
    /// </summary>
    public int MaxConcurrentSessions { get; set; } = 10;

    /// <summary>
    /// Gets or sets the session timeout in minutes.
    /// Default is 30 minutes.
    /// </summary>
    public int SessionTimeoutMinutes { get; set; } = 30;

    /// <summary>
    /// Gets or sets the container image to use for sessions.
    /// Default is "python:3.11-slim".
    /// </summary>
    public string ContainerImage { get; set; } = "python:3.11-slim";
}
