using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models
{
    public class FileUploadResultTests
    {
        [Fact]
        public void Constructor_InitializesRequiredProperties()
        {
            var metadata = new RemoteFileMetadata
            {
                Name = "file.txt",
                LastModifiedAt = DateTime.UtcNow,
                Type = "file"
            };

            var result = new FileUploadResult
            {
                FileMetadata = metadata
            };

            Assert.Equal(metadata, result.FileMetadata);
        }
    }
}
