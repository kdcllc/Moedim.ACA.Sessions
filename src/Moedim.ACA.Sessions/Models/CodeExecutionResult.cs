using System.Text.Json;
using System.Text.Json.Serialization;

namespace Moedim.ACA.Sessions.Models;

/// <summary>
/// Represents the result of a Python code execution.
/// </summary>
public sealed class CodeExecutionResult
{
    /// <summary>
    /// Gets or sets the unique identifier for the execution result.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>
    /// Gets or sets the status of the execution (e.g., Succeeded, Failed).
    /// </summary>
    [JsonPropertyName("status")]
    public required string Status { get; set; }

    /// <summary>
    /// Gets or sets the detailed result of the execution.
    /// </summary>
    [JsonPropertyName("result")]
    public CodeExecutionDetails? Result { get; set; }

    /// <summary>
    /// Returns a string representation of the execution result.
    /// </summary>
    public override string ToString()
    {
        return JsonSerializer.Serialize(new
        {
            id = Id,
            status = Status,
            result = Result?.ExecutionResult,
            stdOut = Result?.StdOut,
            stdErr = Result?.StdErr
        });
    }
}