namespace Moedim.ACA.Sessions.Models;

/// <summary>
/// Represents the result of a file download operation.
/// </summary>
public sealed class FileDownloadResult
{
    /// <summary>
    /// Gets or sets the contents of the downloaded file.
    /// </summary>
    public required byte[] FileContents { get; set; }

    /// <summary>
    /// Gets or sets the name of the remote file.
    /// </summary>
    public required string RemoteFileName { get; set; }
}