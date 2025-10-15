using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models
{
    public class RemoteFileMetadataTests
    {
        [Fact]
        public void Constructor_InitializesRequiredProperties()
        {
            var name = "file.txt";
            var lastModified = DateTime.UtcNow;
            var type = "file";

            var metadata = new RemoteFileMetadata
            {
                Name = name,
                LastModifiedAt = lastModified,
                Type = type
            };

            Assert.Equal(name, metadata.Name);
            Assert.Equal(lastModified, metadata.LastModifiedAt);
            Assert.Equal(type, metadata.Type);
        }
    }
}
