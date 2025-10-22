using System.ComponentModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.Agents;

/// <summary>
/// Plugin that enables code interpretation via ACA Sessions.
/// </summary>
/// <param name="codeInterpreter"></param>
/// <param name="logger"></param>
public class CodeInterpreterPlugin(
    ICodeInterpreter codeInterpreter,
    ILogger<CodeInterpreterPlugin> logger)
{
    /// <summary>
    /// Start and end the code snippet with double quotes to define it as a string.
    /// Insert \n within the string wherever a new line should appear.
    /// Add spaces directly after \n sequences to replicate indentation.
    /// Use \"" to include double quotes within the code without ending the string.
    /// Keep everything in a single line; the \n sequences will represent line breaks
    /// when the string is processed or displayed.
    /// </summary>
    /// <param name="sessionId">The id of the session must be a unique identifier.</param>
    /// <param name="code">The valid Python code to execute.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns> The result of the Python code execution. </returns>
    public async Task<string> RunCodeAsync(
     [Description("The id of the session must be a unique identifier")] string sessionId,
     [Description("The valid Python code to execute.")] string code,
     CancellationToken cancellationToken)
    {
        logger.LogInformation("Executing code for session {SessionId}", sessionId);
        logger.LogDebug("Code to execute: {Code}", code);
        var request = new CodeExecutionRequest
        {
            SessionId = sessionId,
            Code = code,
            SanitizeInput = true
        };

        var result = await codeInterpreter.ExecuteAsync(request, cancellationToken);
        logger.LogInformation("Execution result: {Result}", result.ToString());
        return result.ToString();
    }

    /// <summary>
    /// Returns the functions provided by this plugin.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<AITool> AsAITools()
    {
        yield return AIFunctionFactory.Create(RunCodeAsync);
    }
}
