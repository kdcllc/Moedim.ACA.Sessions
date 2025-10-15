using System.Text.Json.Serialization;

namespace Moedim.ACA.Sessions.Models;

/// <summary>
/// Properties for code execution.
/// </summary>
internal sealed class CodeExecutionProperties
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeExecutionProperties"/> class.
    /// </summary>
    public CodeExecutionProperties(
        string pythonCode,
        int timeoutInSeconds,
        CodeInputType codeInputType,
        CodeExecutionType codeExecutionType)
    {
        PythonCode = pythonCode;
        TimeoutInSeconds = timeoutInSeconds;
        CodeInputType = codeInputType;
        CodeExecutionType = codeExecutionType;
    }

    /// <summary>
    /// Code input type.
    /// </summary>
    [JsonPropertyName("codeInputType")]
    public CodeInputType CodeInputType { get; } = CodeInputType.Inline;

    /// <summary>
    /// Code execution type.
    /// </summary>
    [JsonPropertyName("executionType")]
    public CodeExecutionType CodeExecutionType { get; } = CodeExecutionType.Synchronous;

    /// <summary>
    /// Timeout in seconds for the code execution.
    /// </summary>
    [JsonPropertyName("timeoutInSeconds")]
    public int TimeoutInSeconds { get; } = 100;

    /// <summary>
    /// The Python code to execute.
    /// </summary>
    [JsonPropertyName("code")]
    public string PythonCode { get; }
}