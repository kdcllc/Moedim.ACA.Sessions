namespace Moedim.ACA.Sessions.Models;

/// <summary>
/// Request model for downloading a file from a session.
/// </summary>
public sealed class FileDownloadRequest
{
    /// <summary>
    /// Gets or sets the session identifier.
    /// </summary>
    public required string SessionId { get; set; }

    /// <summary>
    /// Gets or sets the remote file name to download.
    /// </summary>
    public required string RemoteFileName { get; set; }
}