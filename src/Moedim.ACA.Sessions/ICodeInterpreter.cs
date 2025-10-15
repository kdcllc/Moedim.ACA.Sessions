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
    Task<CodeExecutionResult> ExecuteAsync(CodeExecutionRequest req, CancellationToken cancellationToken);
}
