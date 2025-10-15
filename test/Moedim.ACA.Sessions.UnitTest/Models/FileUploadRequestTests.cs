using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models
{
    public class FileUploadRequestTests
    {
        [Fact]
        public void Constructor_InitializesRequiredProperties()
        {
            var sessionId = "session-789";
            var fileName = "upload.txt";
            var fileContent = new byte[] { 1, 2, 3 };

            var request = new FileUploadRequest
            {
                SessionId = sessionId,
                FileName = fileName,
                FileContent = fileContent
            };

            Assert.Equal(sessionId, request.SessionId);
            Assert.Equal(fileName, request.FileName);
            Assert.Equal(fileContent, request.FileContent);
        }
    }
}
