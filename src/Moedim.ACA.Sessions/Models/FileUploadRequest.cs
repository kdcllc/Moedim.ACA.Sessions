namespace Moedim.ACA.Sessions.Models;

/// <summary>
/// Represents a request to upload a file associated with a session.
/// </summary>
public sealed class FileUploadRequest
{
    /// <summary>
    /// Gets or sets the session ID associated with the file upload.
    /// </summary>
    public required string SessionId { get; set; }

    /// <summary>
    /// Gets or sets the name of the file to be uploaded.
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// Gets or sets the content of the file as a byte array.
    /// </summary>
    public required byte[] FileContent { get; set; }
}