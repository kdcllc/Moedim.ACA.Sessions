using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models;

public class CodeExecutionRequestTests
{
[Theory]
[InlineData("session-123", "print('Hello')", true)]
[InlineData("", "", false)]
[InlineData("session-456", "x = 1", false)]
public void Properties_CanBeSetAndRetrieved(string sessionId, string code, bool sanitize)
    {
        // Arrange & Act
        var request = new CodeExecutionRequest
        {
            SessionId = sessionId,
            Code = code,
            SanitizeInput = sanitize
        };

        // Assert
        Assert.Equal(sessionId, request.SessionId);
        Assert.Equal(code, request.Code);
        Assert.Equal(sanitize, request.SanitizeInput);
    }
}
