using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models
{
    public class FileDownloadResultTests
    {
        [Fact]
        public void Constructor_InitializesRequiredProperties()
        {
            var fileContents = new byte[] { 1, 2, 3 };
            var remoteFileName = "download.txt";

            var result = new FileDownloadResult
            {
                FileContents = fileContents,
                RemoteFileName = remoteFileName
            };

            Assert.Equal(fileContents, result.FileContents);
            Assert.Equal(remoteFileName, result.RemoteFileName);
        }
    }
}
