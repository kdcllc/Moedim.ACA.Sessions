using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moedim.ACA.Sessions;
using Xunit;

namespace Moedim.ACA.Sessions.Tests;

public class SessionPoolTests
{
    private readonly ILogger<SessionPool> _logger;
    private readonly SessionPoolOptions _options;

    public SessionPoolTests()
    {
        _logger = new LoggerFactory().CreateLogger<SessionPool>();
        _options = new SessionPoolOptions
        {
            Endpoint = "https://test.azurecontainerapps.io",
            ApiKey = "test-key",
            MaxConcurrentSessions = 5,
            SessionTimeoutMinutes = 10
        };
    }

    [Fact]
    public async Task GetOrCreateSessionAsync_CreatesNewSession()
    {
        // Arrange
        using var pool = new SessionPool(Options.Create(_options), _logger);

        // Act
        var session = await pool.GetOrCreateSessionAsync();

        // Assert
        Assert.NotNull(session);
        Assert.NotEmpty(session.Id);
        Assert.True(session.IsActive);
        Assert.Contains(_options.Endpoint!, session.Endpoint);
    }

    [Fact]
    public async Task GetOrCreateSessionAsync_ReusesActiveSession()
    {
        // Arrange
        using var pool = new SessionPool(Options.Create(_options), _logger);
        var session1 = await pool.GetOrCreateSessionAsync();
        await pool.ReleaseSessionAsync(session1.Id);

        // Act
        var session2 = await pool.GetOrCreateSessionAsync();

        // Assert
        Assert.Equal(session1.Id, session2.Id);
    }

    [Fact]
    public async Task TerminateSessionAsync_RemovesSession()
    {
        // Arrange
        using var pool = new SessionPool(Options.Create(_options), _logger);
        var session = await pool.GetOrCreateSessionAsync();

        // Act
        await pool.TerminateSessionAsync(session.Id);
        var sessions = await pool.GetActiveSessionsAsync();

        // Assert
        Assert.Empty(sessions);
    }

    [Fact]
    public async Task GetActiveSessionsAsync_ReturnsOnlyActiveSessions()
    {
        // Arrange
        using var pool = new SessionPool(Options.Create(_options), _logger);
        var session1 = await pool.GetOrCreateSessionAsync();
        await pool.TerminateSessionAsync(session1.Id);
        var session2 = await pool.GetOrCreateSessionAsync();
        await pool.ReleaseSessionAsync(session2.Id);

        // Act
        var activeSessions = await pool.GetActiveSessionsAsync();

        // Assert
        Assert.Single(activeSessions);
        Assert.Contains(activeSessions, s => s.Id == session2.Id);
    }

    [Fact]
    public async Task ReleaseSessionAsync_KeepsSessionInPool()
    {
        // Arrange
        using var pool = new SessionPool(Options.Create(_options), _logger);
        var session = await pool.GetOrCreateSessionAsync();

        // Act
        await pool.ReleaseSessionAsync(session.Id);
        var sessions = await pool.GetActiveSessionsAsync();

        // Assert
        Assert.Single(sessions);
        Assert.True(session.IsActive);
    }
}
