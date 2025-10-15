using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moedim.ACA.Sessions.Impl;
using Moedim.ACA.Sessions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace Moedim.ACA.Sessions.UnitTest.Impl;

public class SessionsHttpClientTests
{
    private readonly Mock<ILogger<SessionsHttpClient>> _loggerMock = new();
    private readonly Mock<IAzureTokenProvider> _tokenProviderMock = new();
    private readonly Mock<IOptions<SessionsHttpClientOptions>> _optionsMock = new();
    private readonly SessionsHttpClientOptions _options;
    private readonly Uri _endpoint = new("https://api.example.com/");
    private readonly string _apiVersion = "v1";

    public SessionsHttpClientTests()
    {
        _options = new SessionsHttpClientOptions
        {
            Endpoint = _endpoint,
            ApiVersion = _apiVersion
        };
        _optionsMock.Setup(x => x.Value).Returns(_options);
    }

    [Fact(DisplayName = "SendAsync_ValidRequest_ReturnsSuccessResponse")]
    public async Task SendAsync_ValidRequest_ReturnsSuccessResponse()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK));

        using var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateClient(httpClient);
        var response = await client.SendAsync(HttpMethod.Get, "sessions", "session1", CancellationToken.None);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact(DisplayName = "SendAsync_HttpRequestException_ThrowsException")]
    public async Task SendAsync_HttpRequestException_ThrowsException()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        using var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateClient(httpClient);
        await Assert.ThrowsAsync<HttpRequestException>(() => client.SendAsync(HttpMethod.Get, "sessions", "session1", CancellationToken.None));
    }

    [Fact(DisplayName = "SendAsync_NonSuccessStatusCode_ThrowsException")]
    public async Task SendAsync_NonSuccessStatusCode_ThrowsException()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Bad request")
            });

        using var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateClient(httpClient);
        await Assert.ThrowsAsync<HttpRequestException>(() => client.SendAsync(HttpMethod.Get, "sessions", "session1", CancellationToken.None));
    }

    [Fact(DisplayName = "AddHeadersAsync_WithTokenProvider_AddsAuthorizationHeader")]
    public async Task AddHeadersAsync_WithTokenProvider_AddsAuthorizationHeader()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.Headers.Authorization != null), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK));
        _tokenProviderMock.Setup(x => x.GetTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync("token123");

        using var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateClient(httpClient, _tokenProviderMock.Object);
        var response = await client.SendAsync(HttpMethod.Get, "sessions", "session1", CancellationToken.None);
        Assert.NotNull(response);
    }

    [Fact(DisplayName = "AddHeadersAsync_WithoutTokenProvider_DoesNotAddAuthorizationHeader")]
    public async Task AddHeadersAsync_WithoutTokenProvider_DoesNotAddAuthorizationHeader()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(req => req.Headers.Authorization == null), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK));

        using var httpClient = new HttpClient(handlerMock.Object);
        var client = CreateClient(httpClient, null);
        var response = await client.SendAsync(HttpMethod.Get, "sessions", "session1", CancellationToken.None);
        Assert.NotNull(response);
    }

    // Private helper at the end to satisfy SA1202
    private SessionsHttpClient CreateClient(HttpClient httpClient, IAzureTokenProvider? tokenProvider = null)
    {
        return new SessionsHttpClient(httpClient, _optionsMock.Object, tokenProvider ?? _tokenProviderMock.Object, _loggerMock.Object);
    }
}
