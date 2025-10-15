using System.Text.RegularExpressions;

namespace Moedim.ACA.Sessions;

/// <summary>
/// Interpreter for executing code snippets.
/// </summary>
public sealed class CodeInterpreter
{
    private readonly ISessionsHttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeInterpreter"/> class.
    /// </summary>
    /// <param name="httpClient"></param>
    public CodeInterpreter(ISessionsHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private static Regex RemoveLeadingWhitespaceBackticksPython() => new(@"^(\s|`)*(?i:python)?\s*", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

    private static Regex RemoveTrailingWhitespaceBackticks() => new(@"(\s|`)*$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

    /// <summary>
    /// Sanitize input to the python REPL.
    /// Remove whitespace, backtick and "python" (if llm mistakes python console as terminal).
    /// </summary>
    /// <param name="code">The code to sanitize.</param>
    /// <returns>The sanitized code.</returns>
    private static string SanitizeCodeInput(string code)
    {
        // Remove leading whitespace and backticks and python (if llm mistakes python console as terminal)
        code = RemoveLeadingWhitespaceBackticksPython().Replace(code, string.Empty);

        // Remove trailing whitespace and backticks
        code = RemoveTrailingWhitespaceBackticks().Replace(code, string.Empty);

        return code;
    }
}