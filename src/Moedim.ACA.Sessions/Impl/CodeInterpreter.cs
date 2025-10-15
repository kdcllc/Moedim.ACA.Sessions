using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.Impl;

/// <summary>
/// Interpreter for executing code snippets.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CodeInterpreter"/> class.
/// </remarks>
/// <param name="httpClient"></param>
/// <param name="logger"></param>
public sealed class CodeInterpreter(
    ISessionsHttpClient httpClient,
    ILogger<CodeInterpreter> logger) : ICodeInterpreter
{
    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public async Task<FileUploadResult> UploadFileAsync(
        FileUploadRequest req,
        CancellationToken cancellationToken)
    {
        logger.LogTrace("Uploading file {FileName} to session {SessionId}", req.FileName, req.SessionId);

        using var fileContent = new ByteArrayContent(req.FileContent);

        using var multipartFormDataContent = new MultipartFormDataContent()
        {
            { fileContent, "file", req.FileName },
        };

        using var response = await httpClient.SendAsync(
            method: HttpMethod.Post,
            path: $"files",
            sessionId: req.SessionId,
            cancellationToken: cancellationToken,
            httpContent: multipartFormDataContent);

        response.EnsureSuccessStatusCode();

        var cxt = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var fileMetadata = JsonSerializer.Deserialize<RemoteFileMetadata>(cxt)!;

        return new FileUploadResult
        {
            FileMetadata = fileMetadata
        };
    }
}