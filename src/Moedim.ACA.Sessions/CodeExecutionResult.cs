namespace Moedim.ACA.Sessions;

/// <summary>
/// Represents the result of code execution.
/// </summary>
public class CodeExecutionResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the execution was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the standard output from code execution.
    /// </summary>
    public string Output { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the error output from code execution.
    /// </summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the execution time in milliseconds.
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// Gets or sets the session ID used for execution.
    /// </summary>
    public string SessionId { get; set; } = string.Empty;
}
