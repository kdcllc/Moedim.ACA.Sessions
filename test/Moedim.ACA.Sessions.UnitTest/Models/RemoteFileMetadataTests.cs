using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models;

public class RemoteFileMetadataTests
{
    [Theory]
    [InlineData("file.txt", "file")]
    [InlineData("", "")]
    [InlineData("data.csv", "data")]
    [InlineData("image.png", "image")]
    public void Properties_CanBeSetAndRetrieved(string name, string type)
    {
        // Arrange
        var lastModified = DateTime.UtcNow;

        // Act
        var metadata = new RemoteFileMetadata
        {
            Name = name,
            LastModifiedAt = lastModified,
            Type = type
        };

        // Assert
        Assert.Equal(name, metadata.Name);
        Assert.Equal(lastModified, metadata.LastModifiedAt);
        Assert.Equal(type, metadata.Type);
    }

    [Fact]
    public void Properties_CanBeSetAndRetrieved_WithDefaultDate()
    {
        // Arrange
        var name = "default.txt";
        var type = "default";
        var lastModified = default(DateTime);

        // Act
        var metadata = new RemoteFileMetadata
        {
            Name = name,
            LastModifiedAt = lastModified,
            Type = type
        };

        // Assert
        Assert.Equal(name, metadata.Name);
        Assert.Equal(lastModified, metadata.LastModifiedAt);
        Assert.Equal(type, metadata.Type);
    }
}
