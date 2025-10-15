using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models;

public class CodeExecutionTypeTests
{
    [Fact]
    public void Enum_HasExpectedValues()
    {
        Assert.True(Enum.IsDefined(typeof(CodeExecutionType), "Synchronous"));
    }
}
