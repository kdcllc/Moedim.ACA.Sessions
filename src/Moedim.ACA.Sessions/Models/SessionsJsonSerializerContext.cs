#if NET6_0_OR_GREATER
using System.Text.Json.Serialization;

namespace Moedim.ACA.Sessions.Models;

// Register the model types for source-generated JSON serialization so trimming/AOT
// scenarios have the required serialization metadata available.
[JsonSerializable(typeof(CodeExecutionResult))]
[JsonSerializable(typeof(CodeExecutionDetails))]
internal sealed partial class SessionsJsonSerializerContext : JsonSerializerContext
{
}
#endif
