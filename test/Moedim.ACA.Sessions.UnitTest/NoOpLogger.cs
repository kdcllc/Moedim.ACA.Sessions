using Microsoft.Extensions.Logging;

namespace Moedim.ACA.Sessions.UnitTest;

// Minimal no-op logger implementation to avoid bringing logging packages into the tests.
internal sealed class NoOpLogger<T> : ILogger<T>
{
    IDisposable ILogger.BeginScope<TState>(TState state)
    {
        return NoopDisposable.Instance;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return false;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
    }

    private sealed class NoopDisposable : IDisposable
    {
        public static readonly NoopDisposable Instance = new NoopDisposable();

        public void Dispose()
        {
        }
    }
}