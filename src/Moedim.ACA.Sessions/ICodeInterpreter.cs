using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions;

/// <summary>
/// Defines methods for interpreting and executing code snippets.
/// </summary>
public interface ICodeInterpreter
{
    /// <summary>
    /// Executes the provided code asynchronously.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CodeExecutionResult> ExecuteAsync(
        CodeExecutionRequest req,
        CancellationToken cancellationToken);

    /// <summary>
    /// Downloads a file from the session asynchronously.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FileDownloadResult> DownloadFileAsync(
        FileDownloadRequest req,
        CancellationToken cancellationToken);

    /// <summary>
    /// Lists files in the session asynchronously.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<RemoteFileMetadata>> ListFilesAsync(
        string sessionId,
        CancellationToken cancellationToken);
}
