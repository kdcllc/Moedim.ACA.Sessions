using System.Text.Json.Serialization;

namespace Moedim.ACA.Sessions.Models;

/// <summary>
/// Represents the detailed result of a Python code execution.
/// </summary>
public sealed class ExecutionDetails
{
    /// <summary>
    /// Gets or sets the standard output (stdout) of the code execution.
    /// </summary>
    [JsonPropertyName("stdout")]
    public string? StdOut { get; set; }

    /// <summary>
    /// Gets or sets the standard error (stderr) of the code execution.
    /// </summary>
    [JsonPropertyName("stderr")]
    public string? StdErr { get; set; }

    /// <summary>
    /// Gets or sets the result of the code execution.
    /// </summary>
    [JsonPropertyName("executionResult")]
    public string? ExecutionResult { get; set; }
}