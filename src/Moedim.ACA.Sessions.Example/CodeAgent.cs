using Azure.AI.OpenAI;
using Azure.Core;
using Azure.Identity;
using Microsoft.Agents.AI;
using Moedim.ACA.Sessions.Agents;
using OpenAI;

namespace Moedim.ACA.Sessions.Example;

/// <summary>
/// Agent that can execute code via the Code Interpreter plugin.
/// </summary>
public class CodeAgent
{
    private readonly AIAgent _agent;

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeAgent"/> class.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public CodeAgent(IServiceProvider serviceProvider)
    {
        var config = serviceProvider.GetRequiredService<IConfiguration>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var authTokenProvider = serviceProvider.GetRequiredService<IAzureTokenProvider>();

        var endpoint = config["AzureOpenAI:Endpoint"];
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new InvalidOperationException("AzureOpenAI:Endpoint configuration is missing.");
        }

        var deploymentName = config["AzureOpenAI:DeploymentName"];
        if (string.IsNullOrWhiteSpace(deploymentName))
        {
            throw new InvalidOperationException("AzureOpenAI:DeploymentName configuration is missing.");
        }

        // https://cognitiveservices.azure.com/.default
        var accessToken = authTokenProvider.GetTokenAsync(
            ["https://cognitiveservices.azure.com/.default"],
            CancellationToken.None).GetAwaiter().GetResult();

        var creds = new StaticTokenCredential(accessToken);

        _agent = new AzureOpenAIClient(
            new Uri(endpoint),
            creds)
            .GetChatClient(deploymentName)
            .CreateAIAgent(
                instructions: @"
                You are a code execution agent that can run Python code snippets using the Code Interpreter service.
                use the same session for all code execution requests.
                Always use the provided tools to execute code `CodeInterpreterPlugin`.
                When code requires creation of files use the following path `/mnt/data` for creation.",
                name: "CodeExecutionAgent",
                tools: [.. serviceProvider.GetRequiredService<CodeInterpreterPlugin>().AsAITools()],
                services: serviceProvider,
                loggerFactory: loggerFactory);
    }

    /// <summary>
    /// Executes code based on user input.
    /// </summary>
    /// <param name="userInput"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<AgentRunResponse> ExecuteCodeAsync(
        string userInput,
        CancellationToken cancellationToken)
    {
        var response = await _agent.RunAsync(
            userInput,
            cancellationToken: cancellationToken);

        return response;
    }

    private class StaticTokenCredential : TokenCredential
    {
        private readonly AccessToken _accessToken;

        public StaticTokenCredential(AccessToken accessToken)
        {
            _accessToken = accessToken;
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return _accessToken;
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return new ValueTask<AccessToken>(_accessToken);
        }
    }
}
