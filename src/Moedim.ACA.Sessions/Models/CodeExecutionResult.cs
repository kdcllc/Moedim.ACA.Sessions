using System.Text.Json;
using System.Text.Json.Serialization;

namespace Moedim.ACA.Sessions.Models;

/// <summary>
/// Represents the result of a Python code execution.
/// </summary>
public sealed class CodeExecutionResult
{
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
#if NET6_0_OR_GREATER
        // Serialize the model directly using the source-generated context so
        // trimming/AOT scenarios have the required metadata available.
        return JsonSerializer.Serialize(this, SessionsJsonSerializerContext.Default.CodeExecutionResult);
#else
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject();
            writer.WriteString("status", Status);

            writer.WritePropertyName("result");
            if (Result is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteString("executionResult", Result.ExecutionResult);
                writer.WriteString("stdout", Result.StdOut);
                writer.WriteString("stderr", Result.StdErr);
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
#endif
    }
}