# Moedim.ACA.Sessions Sample

This sample demonstrates how to use the Moedim.ACA.Sessions library for Python code execution in Azure Container Apps.

## What This Sample Shows

1. **Basic Python Code Execution**: Execute simple Python code in an isolated container
2. **Python Code with File Uploads**: Execute Python code with file dependencies
3. **Session Management**: View and manage active container sessions

## Running the Sample

```bash
dotnet run
```

## Code Walkthrough

### 1. Configure Services

```csharp
var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

services.AddACASessionPool(options =>
{
    options.Endpoint = "https://example.azurecontainerapps.io";
    options.ApiKey = "your-api-key";
    options.MaxConcurrentSessions = 10;
    options.SessionTimeoutMinutes = 30;
});
```

### 2. Execute Python Code

```csharp
var plugin = serviceProvider.GetRequiredService<PythonCodeInterpreterPlugin>();

var pythonCode = @"
x = 10
y = 20
result = x + y
print(f'The sum of {x} and {y} is {result}')
";

var result = await plugin.ExecutePythonAsync(pythonCode);
Console.WriteLine($"Output: {result.Output}");
```

### 3. Execute with Files

```csharp
var codeWithFiles = @"
import data
print(f'Loaded data: {data.values}')
";

var files = new Dictionary<string, string>
{
    ["data.py"] = "values = [1, 2, 3, 4, 5]"
};

var result = await plugin.ExecutePythonWithFilesAsync(codeWithFiles, files);
```

### 4. Check Active Sessions

```csharp
var sessionPool = serviceProvider.GetRequiredService<ISessionPool>();
var activeSessions = await sessionPool.GetActiveSessionsAsync();

foreach (var session in activeSessions)
{
    Console.WriteLine($"Session ID: {session.Id}");
    Console.WriteLine($"Expires At: {session.ExpiresAt}");
}
```

## Expected Output

```
Azure Container Apps Sessions - Python Code Interpreter Sample
==============================================================

Example 1: Simple Python code execution
Success: True
Output: Code executed successfully in session <session-id>
Execution Time: 14ms
Session ID: <session-id>

Example 2: Python code execution with file uploads
Success: True
Output: Code executed successfully with files: data.py
...

Example 3: Check active sessions
Active Sessions: 1
  - Session ID: <session-id>
    Endpoint: https://example.azurecontainerapps.io/sessions/<session-id>
    ...

Sample completed!
```

## Next Steps

- Integrate with Semantic Kernel for AI-powered applications
- Implement custom error handling
- Add authentication for production environments
- Scale with multiple concurrent sessions
