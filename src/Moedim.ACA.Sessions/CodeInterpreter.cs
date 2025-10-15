using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions;

/// <summary>
/// Interpreter for executing code snippets.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CodeInterpreter"/> class.
/// </remarks>
/// <param name="httpClient"></param>
/// <param name="logger"></param>
internal sealed class CodeInterpreter(
    ISessionsHttpClient httpClient,
    ILogger<CodeInterpreter> logger) : ICodeInterpreter
{
    /// <summary>
    /// Executes the provided code asynchronously.
    /// </summary>
    /// <param name="req">The code execution request.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The result of the code execution.</returns>
    public async Task<CodeExecutionResult> ExecuteAsync(
        CodeExecutionRequest req,
        CancellationToken cancellationToken)
    {
        logger.LogTrace("Executing code in session {SessionId} Python Code: {Code}", req.SessionId, req.Code);

        var requestBody = new CodeExecutionProperties(
                req.Code,
                timeoutInSeconds: req.TimeoutInSeconds,
                codeInputType: req.CodeInputType,
                codeExecutionType: req.CodeExecutionType);

        using var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        using var response = await httpClient.SendAsync(
            method: HttpMethod.Post,
            path: "executions",
            sessionId: req.SessionId,
            cancellationToken: cancellationToken,
            httpContent: content);

        response.EnsureSuccessStatusCode();

        var cxt = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<CodeExecutionResult>(cxt)!;
    }

    /// <summary>
    /// Downloads a file from the session asynchronously.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<FileDownloadResult> DownloadFileAsync(
        FileDownloadRequest req,
        CancellationToken cancellationToken)
    {
        logger.LogTrace("Downloading file {RemoteFileName} from session {SessionId}", req.RemoteFileName, req.SessionId);

        using var response = await httpClient.SendAsync(
            method: HttpMethod.Get,
            path: $"files/{Uri.EscapeDataString(req.RemoteFileName)}/content",
            sessionId: req.SessionId,
            cancellationToken: cancellationToken);

        response.EnsureSuccessStatusCode();

        var fileContents = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);

        return new FileDownloadResult
        {
            FileContents = fileContents,
            RemoteFileName = req.RemoteFileName
        };
    }

    /// <summary>
    /// Lists files in the session asynchronously.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<RemoteFileMetadata>> ListFilesAsync(
        string sessionId,
        CancellationToken cancellationToken)
    {
        logger.LogTrace("Listing files in session {SessionId}", sessionId);

        using var response = await httpClient.SendAsync(
            method: HttpMethod.Get,
            path: $"files",
            sessionId: sessionId,
            cancellationToken: cancellationToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var jsonElementResult = JsonSerializer.Deserialize<JsonElement>(content);

        var files = jsonElementResult.GetProperty("value");

        return files.Deserialize<IReadOnlyList<RemoteFileMetadata>>() ?? Array.Empty<RemoteFileMetadata>();
    }
}