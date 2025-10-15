using Moedim.ACA.Sessions;
using Moedim.ACA.Sessions.Models;

internal sealed class Main : IMain
{
    private readonly ILogger<Main> _logger;
    private readonly ICodeInterpreter _codeInterpreter;
    private readonly IAzureTokenProvider _azureTokenProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public Main(
        ICodeInterpreter codeInterpreter,
        IAzureTokenProvider azureTokenProvider,
        IHostApplicationLifetime applicationLifetime,
        IConfiguration configuration,
        ILogger<Main> logger)
    {
        _codeInterpreter = codeInterpreter;
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

        var cancellationToken = _applicationLifetime.ApplicationStopping;

        var token = await _azureTokenProvider.GetTokenAsync(cancellationToken: cancellationToken);
        _logger.LogInformation("Acquired token with expiry {Token}", token);

        var sessionId = Guid.NewGuid().ToString();

        var req = new CodeExecutionRequest
        {
            SessionId = sessionId,
            Code = "print('Hello, World!')",
            SanitizeInput = true
        };

        _logger.LogInformation("Executing code in session {SessionId}", sessionId);
        var result = await _codeInterpreter.ExecuteAsync(req, cancellationToken);

        return 0;
    }
}
