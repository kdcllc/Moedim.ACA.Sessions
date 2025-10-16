using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Moedim.ACA.Sessions.Models;

/// <summary>
/// Code input type.
/// </summary>
[Description("Code input type.")]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CodeInputType
{
    /// <summary>
    /// Code is provided as a inline string.
    /// </summary>
    [Description("Code is provided as a inline string.")]
    [JsonPropertyName("inline")]
    Inline
}
