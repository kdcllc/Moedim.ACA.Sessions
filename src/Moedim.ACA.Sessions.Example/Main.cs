using Moedim.ACA.Sessions;
using Moedim.ACA.Sessions.Example;
using Moedim.ACA.Sessions.Models;

internal sealed class Main(
    IServiceProvider serviceProvider,
    ICodeInterpreter codeInterpreter,
    IAzureTokenProvider azureTokenProvider,
    IHostApplicationLifetime applicationLifetime,
    IConfiguration configuration,
    ILogger<Main> logger) : IMain
{
    private readonly ILogger<Main> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ICodeInterpreter _codeInterpreter = codeInterpreter ?? throw new ArgumentNullException(nameof(codeInterpreter));
    private readonly IAzureTokenProvider _azureTokenProvider = azureTokenProvider ?? throw new ArgumentNullException(nameof(azureTokenProvider));
    private readonly IHostApplicationLifetime _applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));

    public IConfiguration Configuration { get; set; } = configuration ?? throw new ArgumentNullException(nameof(configuration));

    public async Task<int> RunAsync()
    {
        _logger.LogInformation("Main executed");

        // use this token for stopping the services
        _applicationLifetime.ApplicationStopping.ThrowIfCancellationRequested();

        var cancellationToken = _applicationLifetime.ApplicationStopping;

        var sessionId = Guid.NewGuid().ToString();

        await RunAgentAsync(cancellationToken);

        // await GetTokenProviderAsync(cancellationToken);

        // await HelloWorldAsync(sessionId, cancellationToken);

        // await CreatePythonDeps(sessionId, cancellationToken);

        // await UploadFileAsync(sessionId, cancellationToken);

        // await ListFilesAsync(sessionId, cancellationToken);

        // await DownloadFileAsync(sessionId, cancellationToken);
        return 0;
    }

    private async Task RunAgentAsync(CancellationToken cancellationToken)
    {
        var agent = _serviceProvider.GetRequiredService<CodeAgent>();
        var userInput = "Write a Python function that returns the Fibonacci sequence up to n. and only return a result of the execution of the code";
        var response = await agent.ExecuteCodeAsync(userInput, cancellationToken);

        _logger.LogInformation("Agent response: {Response}", response.ToString());
    }

    private async Task ListFilesAsync(string sessionId, CancellationToken cancellationToken)
    {
        var fileList = await _codeInterpreter.ListFilesAsync(sessionId, cancellationToken);
        _logger.LogInformation("Files in session {SessionId}: {FileList}", sessionId, string.Join(", ", fileList.Select(f => f.Name)));
    }

    private async Task DownloadFileAsync(string sessionId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Downloading installed_packages.json from session {SessionId}", sessionId);
        var downloadedFile = await _codeInterpreter.DownloadFileAsync(
            new FileDownloadRequest
            {
                SessionId = sessionId,
                RemoteFileName = "installed_packages.json"
            },
            cancellationToken);

        var fileContent = System.Text.Encoding.UTF8.GetString(downloadedFile.FileContents);
        _logger.LogInformation("Downloaded file content: {FileContent}", fileContent);
    }

    private async Task UploadFileAsync(string sessionId, CancellationToken cancellationToken)
    {
        // upload a file
        var localFilePath = "README.md";
        _logger.LogInformation("Uploading file {LocalFilePath} to session {SessionId}", localFilePath, sessionId);
        var fileUploadContent = await File.ReadAllBytesAsync(localFilePath, cancellationToken);
        var uploadResult = await _codeInterpreter.UploadFileAsync(
            new FileUploadRequest
            {
                SessionId = sessionId,
                FileName = Path.GetFileName(localFilePath),
                FileContent = fileUploadContent
            },
            cancellationToken);

        _logger.LogInformation("Upload result: {UploadResult}", uploadResult.FileMetadata);
    }

    private async Task GetTokenProviderAsync(CancellationToken cancellationToken)
    {
        var token = await _azureTokenProvider.GetTokenAsync(cancellationToken: cancellationToken);
        _logger.LogInformation("Acquired token with expiry {Token}", token);
    }

    private async Task CreatePythonDeps(string sessionId, CancellationToken cancellationToken)
    {
        var getInstalledPackagesRequest = new CodeExecutionRequest
        {
            SessionId = sessionId,
            Code = "import pkg_resources\nimport json\nimport os\n\n# Define output path\noutput_path = \"/mnt/data/installed_packages.json\"\n\n# Ensure directory exists (helpful if the folder might not exist yet)\nos.makedirs(os.path.dirname(output_path), exist_ok=True)\n\n# Gather package information\npackages = [\n    {\"name\": d.project_name, \"version\": d.version}\n    for d in pkg_resources.working_set\n]\n\n# Save to JSON\nwith open(output_path, \"w\") as f:\n    json.dump(packages, f, indent=4)\n\nprint(f\"Package information saved to {output_path}\")",
            SanitizeInput = true
        };

        _logger.LogInformation("Executing List All Installed Packages code in session {SessionId}", sessionId);
        var listPackagesResult = await _codeInterpreter.ExecuteAsync(getInstalledPackagesRequest, cancellationToken);
        _logger.LogInformation("Execution result for List All Installed Packages code: {Result}", listPackagesResult.ToString());
    }

    private async Task HelloWorldAsync(string sessionId, CancellationToken cancellationToken)
    {
        var helloWorldRequest = new CodeExecutionRequest
        {
            SessionId = sessionId,
            Code = "print('Hello, World!')",
            SanitizeInput = true
        };

        _logger.LogInformation("Executing Hello World code in session {SessionId}", sessionId);
        var result = await _codeInterpreter.ExecuteAsync(helloWorldRequest, cancellationToken);
        _logger.LogInformation("Execution result for Hello World code: {Result}", result.ToString());
    }
}
