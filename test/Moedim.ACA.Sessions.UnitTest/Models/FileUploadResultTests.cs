using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models;

public class FileUploadResultTests
{
    [Fact]
    public void Properties_CanBeSetAndRetrieved_WithMetadata()
    {
        // Arrange
        var metadata = new RemoteFileMetadata
        {
            Name = "file.txt",
            LastModifiedAt = DateTime.UtcNow,
            Type = "file"
        };

        // Act
        var result = new FileUploadResult
        {
            FileMetadata = metadata
        };

        // Assert
        Assert.Equal(metadata, result.FileMetadata);
    }
}
