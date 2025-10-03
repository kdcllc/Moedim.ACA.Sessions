using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Moedim.ACA.Sessions;

// Configure services
var services = new ServiceCollection();

// Add logging
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// Add ACA Sessions with configuration
services.AddACASessionPool(options =>
{
    options.Endpoint = "https://example.azurecontainerapps.io";
    options.ApiKey = "your-api-key";
    options.MaxConcurrentSessions = 10;
    options.SessionTimeoutMinutes = 30;
});

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Get the plugin and session pool
var plugin = serviceProvider.GetRequiredService<PythonCodeInterpreterPlugin>();
var sessionPool = serviceProvider.GetRequiredService<ISessionPool>();

Console.WriteLine("Azure Container Apps Sessions - Python Code Interpreter Sample");
Console.WriteLine("==============================================================\n");

// Example 1: Simple Python execution
Console.WriteLine("Example 1: Simple Python code execution");
var pythonCode = @"
x = 10
y = 20
result = x + y
print(f'The sum of {x} and {y} is {result}')
";

var result = await plugin.ExecutePythonAsync(pythonCode);
Console.WriteLine($"Success: {result.Success}");
Console.WriteLine($"Output: {result.Output}");
Console.WriteLine($"Execution Time: {result.ExecutionTimeMs}ms");
Console.WriteLine($"Session ID: {result.SessionId}\n");

// Example 2: Python execution with files
Console.WriteLine("Example 2: Python code execution with file uploads");
var codeWithFiles = @"
import data
print(f'Loaded data: {data.values}')
print(f'Sum: {sum(data.values)}')
";

var files = new Dictionary<string, string>
{
    ["data.py"] = "values = [1, 2, 3, 4, 5]"
};

var resultWithFiles = await plugin.ExecutePythonWithFilesAsync(codeWithFiles, files);
Console.WriteLine($"Success: {resultWithFiles.Success}");
Console.WriteLine($"Output: {resultWithFiles.Output}");
Console.WriteLine($"Execution Time: {resultWithFiles.ExecutionTimeMs}ms\n");

// Example 3: Get active sessions
Console.WriteLine("Example 3: Check active sessions");
var activeSessions = await sessionPool.GetActiveSessionsAsync();
Console.WriteLine($"Active Sessions: {activeSessions.Count()}");
foreach (var session in activeSessions)
{
    Console.WriteLine($"  - Session ID: {session.Id}");
    Console.WriteLine($"    Endpoint: {session.Endpoint}");
    Console.WriteLine($"    Created At: {session.CreatedAt}");
    Console.WriteLine($"    Expires At: {session.ExpiresAt}");
}

Console.WriteLine("\nSample completed!");

