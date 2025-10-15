internal sealed class Main : IMain
{
    // LoggerMessage delegate for improved performance
    private static readonly Action<ILogger, Exception?> _mainExecutedLog =
        LoggerMessage.Define(
            LogLevel.Information,
            new EventId(1, nameof(Main)),
            "Main executed");

    private readonly ILogger<Main> _logger;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public Main(
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        ILogger<Main> logger)
    {
        _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IConfiguration Configuration { get; set; }

    public Task<int> RunAsync()
    {
        _mainExecutedLog(_logger, null);

        // use this token for stopping the services
        _applicationLifetime.ApplicationStopping.ThrowIfCancellationRequested();

        return Task.FromResult(0);
    }
}
