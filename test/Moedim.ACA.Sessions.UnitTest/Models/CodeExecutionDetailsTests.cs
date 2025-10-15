using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models;

public class CodeExecutionDetailsTests
{
    [Fact]
    public void Constructor_InitializesProperties_DefaultValues()
    {
        // Arrange & Act
        var details = new CodeExecutionDetails();

        // Assert
        Assert.NotNull(details);
        Assert.Null(details.StdOut);
        Assert.Null(details.StdErr);
        Assert.Null(details.ExecutionResult);
    }

    [Theory]
    [InlineData("output1")]
    [InlineData("")]
    [InlineData(null)]
    public void StdOut_SetValue_ValueIsSet(string? value)
    {
        // Arrange
        var details = new CodeExecutionDetails();

        // Act
        details.StdOut = value;

        // Assert
        Assert.Equal(value, details.StdOut);
    }

    [Theory]
    [InlineData("error1")]
    [InlineData("")]
    [InlineData(null)]
    public void StdErr_SetValue_ValueIsSet(string? value)
    {
        // Arrange
        var details = new CodeExecutionDetails();

        // Act
        details.StdErr = value;

        // Assert
        Assert.Equal(value, details.StdErr);
    }

    [Theory]
    [InlineData("result1")]
    [InlineData("")]
    [InlineData(null)]
    public void ExecutionResult_SetValue_ValueIsSet(string? value)
    {
        // Arrange
        var details = new CodeExecutionDetails();

        // Act
        details.ExecutionResult = value;

        // Assert
        Assert.Equal(value, details.ExecutionResult);
    }
}
