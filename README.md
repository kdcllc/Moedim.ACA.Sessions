# Moedim.ACA.Sessions

Library for dynamic sessions in Azure Container Apps with Python Code Interpreter support.

## Overview

Moedim.ACA.Sessions is a .NET library that provides a managed session pool for Azure Container Apps (ACA) with integrated Python code interpreter capabilities. Built on top of Microsoft Semantic Kernel, it enables safe and isolated code execution in containerized environments.

## Features

- **Session Pool Management**: Efficiently manage multiple Azure Container Apps sessions
- **Python Code Interpreter**: Execute Python code in isolated container environments
- **Semantic Kernel Integration**: Built as a Semantic Kernel plugin for AI-powered applications
- **Dependency Injection Support**: First-class support for .NET dependency injection
- **Configurable**: Flexible configuration options for endpoints, timeouts, and concurrency
- **File Upload Support**: Execute Python code with file dependencies

## Installation

```bash
dotnet add package Moedim.ACA.Sessions
```

## Quick Start

### Basic Configuration

```csharp
using Microsoft.Extensions.DependencyInjection;
using Moedim.ACA.Sessions;

var services = new ServiceCollection();

// Add ACA Sessions with configuration
services.AddACASessionPool(options =>
{
    options.Endpoint = "https://your-aca-endpoint.azurecontainerapps.io";
    options.ApiKey = "your-api-key";
    options.MaxConcurrentSessions = 10;
    options.SessionTimeoutMinutes = 30;
});

var serviceProvider = services.BuildServiceProvider();
```

### Execute Python Code

```csharp
var plugin = serviceProvider.GetRequiredService<PythonCodeInterpreterPlugin>();

var pythonCode = @"
x = 10
y = 20
result = x + y
print(f'The sum of {x} and {y} is {result}')
";

var result = await plugin.ExecutePythonAsync(pythonCode);
Console.WriteLine($"Success: {result.Success}");
Console.WriteLine($"Output: {result.Output}");
```

### Execute Python Code with Files

```csharp
var codeWithFiles = @"
import data
print(f'Loaded data: {data.values}')
print(f'Sum: {sum(data.values)}')
";

var files = new Dictionary<string, string>
{
    ["data.py"] = "values = [1, 2, 3, 4, 5]"
};

var result = await plugin.ExecutePythonWithFilesAsync(codeWithFiles, files);
```

### Use with Semantic Kernel

```csharp
using Microsoft.SemanticKernel;

var kernel = Kernel.CreateBuilder()
    .Build();

var plugin = serviceProvider.GetRequiredService<PythonCodeInterpreterPlugin>();
kernel.Plugins.AddFromObject(plugin, "PythonInterpreter");

// Now you can use it in your AI workflows
```

## Configuration Options

| Option | Description | Default |
|--------|-------------|---------|
| `Endpoint` | Azure Container Apps endpoint URL | - |
| `ApiKey` | API key for authentication | - |
| `MaxConcurrentSessions` | Maximum number of concurrent sessions | 10 |
| `SessionTimeoutMinutes` | Session timeout in minutes | 30 |
| `ContainerImage` | Container image for Python execution | "python:3.11-slim" |

## API Reference

### ISessionPool

The session pool interface for managing container sessions.

#### Methods

- `Task<SessionInfo> GetOrCreateSessionAsync(CancellationToken)` - Get or create a session
- `Task ReleaseSessionAsync(string sessionId, CancellationToken)` - Release a session back to the pool
- `Task TerminateSessionAsync(string sessionId, CancellationToken)` - Terminate a session
- `Task<IEnumerable<SessionInfo>> GetActiveSessionsAsync(CancellationToken)` - Get all active sessions

### PythonCodeInterpreterPlugin

Semantic Kernel plugin for Python code execution.

#### Methods

- `Task<CodeExecutionResult> ExecutePythonAsync(string code, CancellationToken)` - Execute Python code
- `Task<CodeExecutionResult> ExecutePythonWithFilesAsync(string code, Dictionary<string, string> files, CancellationToken)` - Execute Python code with file uploads

## Architecture

This library is based on the [CodeInterpreter plugin from Semantic Kernel](https://github.com/kdcllc/semantic-kernel/blob/90d158cbf8bd4598159a6fe64df745e56d9cbdf4/dotnet/src/Plugins/Plugins.Core/CodeInterpreter) and adapted for Azure Container Apps Sessions.

### Components

1. **SessionPool**: Manages a pool of container sessions for efficient resource utilization
2. **PythonCodeInterpreterPlugin**: Semantic Kernel plugin that provides code execution capabilities
3. **SessionInfo**: Represents metadata about a container session
4. **CodeExecutionResult**: Contains the result of code execution

## Examples

See the [sample project](samples/Moedim.ACA.Sessions.Sample) for a complete working example.

## Building from Source

```bash
git clone https://github.com/kdcllc/Moedim.ACA.Sessions.git
cd Moedim.ACA.Sessions
dotnet build
```

## Running Tests

```bash
dotnet test
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Based on the [Semantic Kernel CodeInterpreter plugin](https://github.com/kdcllc/semantic-kernel)
- Built for [Azure Container Apps](https://azure.microsoft.com/products/container-apps/)

