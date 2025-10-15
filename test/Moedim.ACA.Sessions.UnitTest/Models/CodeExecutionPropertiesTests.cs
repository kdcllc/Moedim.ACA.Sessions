using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models
{
    public class CodeExecutionPropertiesTests
    {
        [Fact]
        public void Constructor_InitializesPropertiesCorrectly()
        {
            var pythonCode = "print('Hello World')";
            var timeout = 120;
            var inputType = CodeInputType.Inline;
            var execType = CodeExecutionType.Synchronous;

            var props = new CodeExecutionProperties(pythonCode, timeout, inputType, execType);

            Assert.Equal(pythonCode, props.PythonCode);
            Assert.Equal(timeout, props.TimeoutInSeconds);
            Assert.Equal(inputType, props.CodeInputType);
            Assert.Equal(execType, props.CodeExecutionType);
        }
    }
}
