using Moedim.ACA.Sessions.Models;

namespace Moedim.ACA.Sessions.UnitTest.Models;

public class FileDownloadResultTests
{
    [Theory]
    [InlineData(new byte[] { 1, 2, 3 }, "download.txt")]
    [InlineData(new byte[] { }, "")]
    [InlineData(new byte[] { 255, 0, 127 }, "data.bin")]
    [InlineData(new byte[] { 42 }, "file42.dat")]
    public void Properties_CanBeSetAndRetrieved(byte[] fileContents, string remoteFileName)
    {
        // Arrange & Act
        var result = new FileDownloadResult
        {
            FileContents = fileContents,
            RemoteFileName = remoteFileName
        };

        // Assert
        Assert.Equal(fileContents, result.FileContents);
        Assert.Equal(remoteFileName, result.RemoteFileName);
    }
}
