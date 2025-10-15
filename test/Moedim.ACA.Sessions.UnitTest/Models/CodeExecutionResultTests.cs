using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models
{
    public class CodeExecutionResultTests
    {
        [Fact]
        public void Constructor_InitializesRequiredProperties()
        {
            var id = "result-123";
            var status = "Succeeded";

            var result = new CodeExecutionResult
            {
                Id = id,
                Status = status
            };

            Assert.Equal(id, result.Id);
            Assert.Equal(status, result.Status);
            Assert.Null(result.Result);
        }
    }
}
