using System.Text.RegularExpressions;

namespace Moedim.ACA.Sessions.Models;

/// <summary>
/// Represents a request to execute a code snippet.
/// </summary>
public sealed class CodeExecutionRequest
{
    /// <summary>
    /// Gets or sets the unique session identifier.
    /// </summary>
    public required string SessionId { get; set; }

    /// <summary>
    /// Gets or sets the code snippet to be executed.
    /// </summary>
    public required string Code
    {
        get;
        set => field = value is null ? string.Empty : SanitizeCodeInput(value, SanitizeInput);
    }

    /// <summary>
    /// Sanitize input code by removing leading/trailing whitespace and backticks.
    /// </summary>
    public required bool SanitizeInput { get; set; } = true;

    /// <summary>
    /// Code input type.
    /// </summary>
    public CodeInputType CodeInputType { get; } = CodeInputType.Inline;

    /// <summary>
    /// Code execution type.
    /// </summary>
    public CodeExecutionType CodeExecutionType { get; } = CodeExecutionType.Synchronous;

    /// <summary>
    /// Timeout in seconds for the code execution.
    /// </summary>
    public int TimeoutInSeconds { get; } = 100;

    private static Regex RemoveLeadingWhitespaceBackticksPython() => new(@"^(\s|`)*(?i:python)?\s*", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

    private static Regex RemoveTrailingWhitespaceBackticks() => new(@"(\s|`)*$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

    /// <summary>
    /// Sanitize input to the python REPL.
    /// Remove whitespace, backtick and "python" (if llm mistakes python console as terminal).
    /// </summary>
    /// <param name="code">The code to sanitize.</param>
    /// <param name="sanitize"></param>
    /// <returns>The sanitized code.</returns>
    private static string SanitizeCodeInput(string code, bool sanitize)
    {
        if (!sanitize)
        {
            return code;
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return string.Empty;
        }

        // Remove leading whitespace and backticks and python (if llm mistakes python console as terminal)
        code = RemoveLeadingWhitespaceBackticksPython().Replace(code, string.Empty);

        // Remove trailing whitespace and backticks
        code = RemoveTrailingWhitespaceBackticks().Replace(code, string.Empty);

        return code;
    }
}