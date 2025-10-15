using Moedim.ACA.Sessions.Options;

namespace Moedim.ACA.Sessions.UnitTest;

public class SessionsHttpClientOptionsTests
{
    [Fact]
    public void Endpoint_IsNull_WhenNotSet()
    {
        var opts = new SessionsHttpClientOptions();
        Assert.Null(opts.Endpoint);
    }

    [Fact]
    public void Endpoint_RemovesPythonExecuteAndAddsTrailingSlash_WhenSetToExecuteUri()
    {
        var opts = new SessionsHttpClientOptions
        {
            Endpoint = new Uri("https://example.com/python/execute")
        };
        Assert.Equal(new Uri("https://example.com/"), opts.Endpoint);
    }

    [Fact]
    public void Endpoint_AppendsTrailingSlash_WhenMissing()
    {
        var opts = new SessionsHttpClientOptions
        {
            Endpoint = new Uri("https://example.com")
        };
        Assert.Equal(new Uri("https://example.com/"), opts.Endpoint);
    }

    [Fact]
    public void Endpoint_KeepsTrailingSlash_WhenPresent()
    {
        var opts = new SessionsHttpClientOptions
        {
            Endpoint = new Uri("https://example.com/")
        };
        Assert.Equal(new Uri("https://example.com/"), opts.Endpoint);
    }
}
