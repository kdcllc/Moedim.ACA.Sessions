using System.Text;
using System.Text.Json;
using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions;

/// <summary>
/// Interpreter for executing code snippets.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CodeInterpreter"/> class.
/// </remarks>
/// <param name="httpClient"></param>
public sealed class CodeInterpreter(ISessionsHttpClient httpClient) : ICodeInterpreter
{
    private readonly ISessionsHttpClient _httpClient = httpClient;

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
        var requestBody = new CodeExecutionProperties(
                req.Code,
                timeoutInSeconds: req.TimeoutInSeconds,
                codeInputType: req.CodeInputType,
                codeExecutionType: req.CodeExecutionType);

        using var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(
            method: HttpMethod.Post,
            path: "executions",
            sessionId: req.SessionId,
            cancellationToken: cancellationToken,
            httpContent: content);

        response.EnsureSuccessStatusCode();

        var cxt = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<CodeExecutionResult>(cxt)!;
    }
}