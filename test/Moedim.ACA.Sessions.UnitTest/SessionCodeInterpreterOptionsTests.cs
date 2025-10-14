namespace Moedim.ACA.Sessions.UnitTest;

public class SessionCodeInterpreterOptionsTests
{
    [Fact]
    public void BaseEndpoint_IsNull_WhenEndpointIsNull()
    {
        var opts = new Options.SessionCodeInterpreterOptions();
        Assert.Null(opts.BaseEndpoint);
    }

    [Fact]
    public void BaseEndpoint_RemovesPythonExecuteAndAddsTrailingSlash()
    {
        var opts = new Options.SessionCodeInterpreterOptions
        {
            Endpoint = new Uri("https://example.com/python/execute")
        };

        Assert.Equal(new Uri("https://example.com/"), opts.BaseEndpoint);
    }

    [Fact]
    public void BaseEndpoint_AppendsTrailingSlash_WhenMissing()
    {
        var opts = new Options.SessionCodeInterpreterOptions
        {
            Endpoint = new Uri("https://example.com")
        };

        Assert.Equal(new Uri("https://example.com/"), opts.BaseEndpoint);
    }

    [Fact]
    public void BaseEndpoint_KeepsTrailingSlash_WhenPresent()
    {
        var opts = new Options.SessionCodeInterpreterOptions
        {
            Endpoint = new Uri("https://example.com/")
        };

        Assert.Equal(new Uri("https://example.com/"), opts.BaseEndpoint);
    }
}
