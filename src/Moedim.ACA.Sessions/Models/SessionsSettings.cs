using System.Text.Json.Serialization;

namespace Moedim.ACA.Sessions.Models;

/// <summary>
/// Settings for a Python Sessions Plugin.
/// </summary>
public class SessionsSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SessionsSettings"/> class.
    /// </summary>
    /// <param name="sessionId">Session identifier.</param>
    /// <param name="endpoint">Azure Container Apps Endpoint.</param>
    [JsonConstructor]
    public SessionsSettings(string sessionId, Uri endpoint)
    {
        SessionId = sessionId;
        Endpoint = endpoint;
    }

    /// <summary>
    /// Determines if the input should be sanitized.
    /// </summary>
    [JsonIgnore]
    public bool SanitizeInput { get; set; }

    /// <summary>
    /// The target endpoint.
    /// </summary>
    [JsonIgnore]
    public Uri Endpoint { get; set; }

    /// <summary>
    /// List of allowed domains to download from.
    /// </summary>
    public IEnumerable<string>? AllowedDomains { get; set; }

    /// <summary>
    /// The session identifier.
    /// </summary>
    [JsonPropertyName("identifier")]
    public string SessionId { get; set; }

    /// <summary>
    /// Code input type.
    /// </summary>
    [JsonPropertyName("codeInputType")]
    public CodeInputType CodeInputType { get; set; } = CodeInputType.Inline;

    /// <summary>
    /// Code execution type.
    /// </summary>
    [JsonPropertyName("executionType")]
    public CodeExecutionType CodeExecutionType { get; set; } = CodeExecutionType.Synchronous;

    /// <summary>
    /// Timeout in seconds for the code execution.
    /// </summary>
    [JsonPropertyName("timeoutInSeconds")]
    public int TimeoutInSeconds { get; set; } = 100;
}