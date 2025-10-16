using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models;

public class FileUploadRequestTests
{
    [Theory]
    [InlineData("session-789", "upload.txt", new byte[] { 1, 2, 3 })]
    [InlineData("", "", new byte[] { })]
    [InlineData("session-000", "image.png", new byte[] { 42 })]
    [InlineData("session-123", "data.csv", new byte[] { 255, 0, 127 })]
    public void Properties_CanBeSetAndRetrieved(string sessionId, string fileName, byte[] fileContent)
    {
        // Arrange & Act
        var request = new FileUploadRequest
        {
            SessionId = sessionId,
            FileName = fileName,
            FileContent = fileContent
        };

        // Assert
        Assert.Equal(sessionId, request.SessionId);
        Assert.Equal(fileName, request.FileName);
        Assert.Equal(fileContent, request.FileContent);
    }
}
