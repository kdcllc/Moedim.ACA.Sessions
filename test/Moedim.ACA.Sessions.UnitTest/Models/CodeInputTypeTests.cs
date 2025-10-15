using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models
{
    public class CodeInputTypeTests
    {
        [Fact]
        public void Enum_HasExpectedValues()
        {
            Assert.True(Enum.IsDefined(typeof(CodeInputType), "Inline"));
        }
    }
}
