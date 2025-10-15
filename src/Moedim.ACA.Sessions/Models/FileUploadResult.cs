namespace Moedim.ACA.Sessions.Models;

/// <summary>
/// Represents the result of a file upload operation.
/// </summary>
public sealed class FileUploadResult
{
    /// <summary>
    /// Gets or sets the metadata of the uploaded file.
    /// </summary>
    public required RemoteFileMetadata FileMetadata { get; set; }
}