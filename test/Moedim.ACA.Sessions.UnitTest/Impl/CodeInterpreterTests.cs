using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moedim.ACA.Sessions.Impl;
using Moedim.ACA.Sessions.Models;
using Moq;

namespace Moedim.ACA.Sessions.UnitTest.Impl;

public class CodeInterpreterTests
{
    private readonly Mock<ISessionsHttpClient> _httpClientMock = new();
    private readonly Mock<ILogger<CodeInterpreter>> _loggerMock = new();
    private readonly CodeInterpreter _interpreter;

    public CodeInterpreterTests()
    {
        _interpreter = new CodeInterpreter(_httpClientMock.Object, _loggerMock.Object);
    }

    [Fact(DisplayName = "ExecuteAsync_ValidRequest_ReturnsCodeExecutionResult")]
    public async Task ExecuteAsync_ValidRequest_ReturnsCodeExecutionResult()
    {
        var req = new CodeExecutionRequest
        {
            SessionId = "session1",
            Code = "print('Hello')",
            SanitizeInput = true
        };
        var expectedResult = new CodeExecutionResult
        {
            Id = "result1",
            Status = "Succeeded",
            Result = null
        };
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(expectedResult), Encoding.UTF8, "application/json")
        };
        _httpClientMock.Setup(x => x.SendAsync(
            HttpMethod.Post,
            "executions",
            req.SessionId,
            It.IsAny<CancellationToken>(),
            It.IsAny<HttpContent>()))
            .ReturnsAsync(response);

        var result = await _interpreter.ExecuteAsync(req, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal("result1", result.Id);
        Assert.Equal("Succeeded", result.Status);
    }

    [Fact(DisplayName = "DownloadFileAsync_ValidRequest_ReturnsFileDownloadResult")]
    public async Task DownloadFileAsync_ValidRequest_ReturnsFileDownloadResult()
    {
        var req = new FileDownloadRequest { SessionId = "session1", RemoteFileName = "file.txt" };
        var fileBytes = Encoding.UTF8.GetBytes("file content");
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(fileBytes)
        };
        _httpClientMock.Setup(x => x.SendAsync(
            HttpMethod.Get,
            It.Is<string>(p => p.Contains("file.txt")),
            req.SessionId,
            It.IsAny<CancellationToken>(),
            null))
            .ReturnsAsync(response);

        var result = await _interpreter.DownloadFileAsync(req, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(fileBytes, result.FileContents);
        Assert.Equal("file.txt", result.RemoteFileName);
    }

    [Fact(DisplayName = "ListFilesAsync_ValidSessionId_ReturnsFileList")]
    public async Task ListFilesAsync_ValidSessionId_ReturnsFileList()
    {
        var sessionId = "session1";
        var files = new[]
        {
            new RemoteFileMetadata
            {
                Name = "file1.txt",
                LastModifiedAt = DateTime.UtcNow,
                Type = "file"
            }
        };
        var json = JsonSerializer.Serialize(new { value = files });
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        _httpClientMock.Setup(x => x.SendAsync(
            HttpMethod.Get,
            "files",
            sessionId,
            It.IsAny<CancellationToken>(),
            null))
            .ReturnsAsync(response);

        var result = await _interpreter.ListFilesAsync(sessionId, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("file1.txt", result[0].Name);
    }

    [Fact(DisplayName = "UploadFileAsync_ValidRequest_ReturnsFileUploadResult")]
    public async Task UploadFileAsync_ValidRequest_ReturnsFileUploadResult()
    {
        var req = new FileUploadRequest
        {
            SessionId = "session1",
            FileName = "file.txt",
            FileContent = Encoding.UTF8.GetBytes("file content")
        };
        var metadata = new RemoteFileMetadata
        {
            Name = "file.txt",
            LastModifiedAt = DateTime.UtcNow,
            Type = "file"
        };
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(metadata), Encoding.UTF8, "application/json")
        };
        _httpClientMock.Setup(x => x.SendAsync(
            HttpMethod.Post,
            "files",
            req.SessionId,
            It.IsAny<CancellationToken>(),
            It.IsAny<MultipartFormDataContent>()))
            .ReturnsAsync(response);

        var result = await _interpreter.UploadFileAsync(req, CancellationToken.None);
        Assert.NotNull(result);
        Assert.NotNull(result.FileMetadata);
        Assert.Equal("file.txt", result.FileMetadata.Name);
    }
}
