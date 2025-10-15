using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models;

public class FileDownloadRequestTests
{
    [Theory]
    [InlineData("session-456", "file.txt")]
    [InlineData("", "")]
    [InlineData("session-789", "data.csv")]
    [InlineData("session-000", "image.png")]
    public void Properties_CanBeSetAndRetrieved(string sessionId, string remoteFileName)
    {
        // Arrange & Act
        var request = new FileDownloadRequest
        {
            SessionId = sessionId,
            RemoteFileName = remoteFileName
        };

        // Assert
        Assert.Equal(sessionId, request.SessionId);
        Assert.Equal(remoteFileName, request.RemoteFileName);
    }
}
