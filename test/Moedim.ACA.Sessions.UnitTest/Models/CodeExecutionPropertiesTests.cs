using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models;

public class CodeExecutionPropertiesTests
{
[Theory]
[InlineData("print('Hello World')", 120)]
[InlineData("", 0)]
[InlineData("x = 1", -1)]
[InlineData("import sys", int.MaxValue)]
public void Constructor_InitializesProperties_WithVariousInputs(string pythonCode, int timeout)
    {
        // Arrange
        var inputType = CodeInputType.Inline;
        var execType = CodeExecutionType.Synchronous;

        // Act
        var props = new CodeExecutionProperties(pythonCode, timeout, inputType, execType);

        // Assert
        Assert.Equal(pythonCode, props.PythonCode);
        Assert.Equal(timeout, props.TimeoutInSeconds);
        Assert.Equal(inputType, props.CodeInputType);
        Assert.Equal(execType, props.CodeExecutionType);
    }

[Fact]
public void Constructor_InitializesPropertiesCorrectly_Defaults()
    {
        // Arrange
        var pythonCode = "print('Hello World')";
        var timeout = 100;
        var inputType = CodeInputType.Inline;
        var execType = CodeExecutionType.Synchronous;

        // Act
        var props = new CodeExecutionProperties(pythonCode, timeout, inputType, execType);

        // Assert
        Assert.Equal(pythonCode, props.PythonCode);
        Assert.Equal(timeout, props.TimeoutInSeconds);
        Assert.Equal(CodeInputType.Inline, props.CodeInputType);
        Assert.Equal(CodeExecutionType.Synchronous, props.CodeExecutionType);
    }
}
