using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models;

public class CodeExecutionResultTests
{
    [Fact]
    public void Properties_CanBeSetAndRetrieved_WithNullResult()
    {
        // Arrange
        var id = "result-123";
        var status = "Succeeded";

        // Act
        var result = new CodeExecutionResult
        {
            Id = id,
            Status = status,
            Result = null
        };

        // Assert
        Assert.Equal(id, result.Id);
        Assert.Equal(status, result.Status);
        Assert.Null(result.Result);
    }

    [Fact]
    public void Properties_CanBeSetAndRetrieved_WithCodeExecutionDetailsResult()
    {
        // Arrange
        var id = "result-456";
        var status = "Failed";
        var details = new CodeExecutionDetails
        {
            StdOut = "output",
            StdErr = "error",
            ExecutionResult = "result"
        };

        // Act
        var result = new CodeExecutionResult
        {
            Id = id,
            Status = status,
            Result = details
        };

        // Assert
        Assert.Equal(id, result.Id);
        Assert.Equal(status, result.Status);
        Assert.NotNull(result.Result);
        Assert.Equal("output", result.Result.StdOut);
        Assert.Equal("error", result.Result.StdErr);
        Assert.Equal("result", result.Result.ExecutionResult);
    }
}
