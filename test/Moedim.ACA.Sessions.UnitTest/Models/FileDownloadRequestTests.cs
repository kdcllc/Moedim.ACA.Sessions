using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models
{
    public class FileDownloadRequestTests
    {
        [Fact]
        public void Constructor_InitializesRequiredProperties()
        {
            var sessionId = "session-456";
            var remoteFileName = "file.txt";

            var request = new FileDownloadRequest
            {
                SessionId = sessionId,
                RemoteFileName = remoteFileName
            };

            Assert.Equal(sessionId, request.SessionId);
            Assert.Equal(remoteFileName, request.RemoteFileName);
        }
    }
}
