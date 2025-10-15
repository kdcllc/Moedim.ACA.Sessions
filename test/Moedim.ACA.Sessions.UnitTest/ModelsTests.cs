using System.Text.Json;
using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest;

public class ModelsTests
{
    // Verifies that ToString() includes the expected properties when Result is null.
    [Fact]
    public void ToString_IncludesExpectedProperties_WithNullResult()
    {
        var result = new CodeExecutionResult
        {
            Id = "123",
            Status = "Succeeded",
            Result = null
        };

        var json = result.ToString();

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal("Succeeded", root.GetProperty("status").GetString());
        Assert.Equal(JsonValueKind.Null, root.GetProperty("result").ValueKind);

        // After switching to serializing the model directly, stdout/stderr are nested
        // under "result" when present. They should not be top-level properties.
        Assert.False(root.TryGetProperty("stdOut", out _));
        Assert.False(root.TryGetProperty("stdErr", out _));
    }

    // Verifies that ToString() serializes the nested CodeExecutionDetails values.
    [Fact]
    public void ToString_SerializesResultAndStdOutStdErr()
    {
        var details = new CodeExecutionDetails
        {
            ExecutionResult = "42",
            StdOut = "Hello",
            StdErr = "Err"
        };

        var result = new CodeExecutionResult
        {
            Id = "123",
            Status = "Failed",
            Result = details
        };

        var json = result.ToString();

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal("Failed", root.GetProperty("status").GetString());

        var resultElement = root.GetProperty("result");
        Assert.Equal(JsonValueKind.Object, resultElement.ValueKind);
        Assert.Equal("42", resultElement.GetProperty("executionResult").GetString());
        Assert.Equal("Hello", resultElement.GetProperty("stdout").GetString());
        Assert.Equal("Err", resultElement.GetProperty("stderr").GetString());
    }
}
