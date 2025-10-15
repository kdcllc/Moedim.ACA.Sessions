using Moedim.ACA.Sessions;

internal sealed class Main : IMain
{
    private readonly ILogger<Main> _logger;
    private readonly IAzureTokenProvider _azureTokenProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public Main(
        IAzureTokenProvider azureTokenProvider,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        ILogger<Main> logger)
    {
        _azureTokenProvider = azureTokenProvider ?? throw new ArgumentNullException(nameof(azureTokenProvider));
        _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IConfiguration Configuration { get; set; }

    public async Task<int> RunAsync()
    {
        _logger.LogInformation("Main executed");

        // use this token for stopping the services
        _applicationLifetime.ApplicationStopping.ThrowIfCancellationRequested();

        var token = await _azureTokenProvider.GetTokenAsync(cancellationToken: _applicationLifetime.ApplicationStopping);
        _logger.LogInformation("Acquired token with expiry {Token}", token);

        return 0;
    }
}
