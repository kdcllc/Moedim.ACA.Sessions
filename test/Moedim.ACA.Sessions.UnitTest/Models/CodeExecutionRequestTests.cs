using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models
{
    public class CodeExecutionRequestTests
    {
        [Fact]
        public void Constructor_InitializesRequiredProperties()
        {
            var sessionId = "session-123";
            var code = "print('Hello')";
            var sanitize = true;

            var request = new CodeExecutionRequest
            {
                SessionId = sessionId,
                Code = code,
                SanitizeInput = sanitize
            };

            Assert.Equal(sessionId, request.SessionId);
            Assert.Equal("print('Hello')", request.Code);
            Assert.True(request.SanitizeInput);
        }
    }
}
