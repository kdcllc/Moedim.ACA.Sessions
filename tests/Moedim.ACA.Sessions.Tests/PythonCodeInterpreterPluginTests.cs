using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moedim.ACA.Sessions;
using Xunit;

namespace Moedim.ACA.Sessions.Tests;

public class PythonCodeInterpreterPluginTests
{
    private readonly ILogger<PythonCodeInterpreterPlugin> _logger;
    private readonly ILogger<SessionPool> _poolLogger;
    private readonly SessionPoolOptions _options;

    public PythonCodeInterpreterPluginTests()
    {
        var loggerFactory = new LoggerFactory();
        _logger = loggerFactory.CreateLogger<PythonCodeInterpreterPlugin>();
        _poolLogger = loggerFactory.CreateLogger<SessionPool>();
        _options = new SessionPoolOptions
        {
            Endpoint = "https://test.azurecontainerapps.io",
            ApiKey = "test-key"
        };
    }

    [Fact]
    public async Task ExecutePythonAsync_ReturnsSuccessResult()
    {
        // Arrange
        using var pool = new SessionPool(Options.Create(_options), _poolLogger);
        var plugin = new PythonCodeInterpreterPlugin(pool, _logger);
        var code = "print('Hello, World!')";

        // Act
        var result = await plugin.ExecutePythonAsync(code);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotEmpty(result.Output);
        Assert.NotEmpty(result.SessionId);
        Assert.True(result.ExecutionTimeMs >= 0);
    }

    [Fact]
    public async Task ExecutePythonWithFilesAsync_ReturnsSuccessResult()
    {
        // Arrange
        using var pool = new SessionPool(Options.Create(_options), _poolLogger);
        var plugin = new PythonCodeInterpreterPlugin(pool, _logger);
        var code = "import data; print(data.value)";
        var files = new Dictionary<string, string>
        {
            ["data.py"] = "value = 'test'"
        };

        // Act
        var result = await plugin.ExecutePythonWithFilesAsync(code, files);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotEmpty(result.Output);
        Assert.Contains("data.py", result.Output);
        Assert.NotEmpty(result.SessionId);
    }

    [Fact]
    public async Task ExecutePythonAsync_ContainsCodeInOutput()
    {
        // Arrange
        using var pool = new SessionPool(Options.Create(_options), _poolLogger);
        var plugin = new PythonCodeInterpreterPlugin(pool, _logger);
        var code = "x = 42";

        // Act
        var result = await plugin.ExecutePythonAsync(code);

        // Assert
        Assert.Contains(code, result.Output);
    }

    [Fact]
    public async Task ExecutePythonAsync_ReleasesSession()
    {
        // Arrange
        using var pool = new SessionPool(Options.Create(_options), _poolLogger);
        var plugin = new PythonCodeInterpreterPlugin(pool, _logger);

        // Act
        await plugin.ExecutePythonAsync("print('test')");
        var sessions = await pool.GetActiveSessionsAsync();

        // Assert
        Assert.Single(sessions); // Session should still be in pool
    }
}
