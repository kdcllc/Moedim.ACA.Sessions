
# Moedim.ACA.Sessions

Moedim.ACA.Sessions is a .NET library designed to simplify secure code execution, session management, and Azure token handling for cloud-native applications. It provides abstractions and implementations for interacting with remote code execution services, managing authentication tokens, and handling file uploads/downloads in distributed environments.

## Features

- Secure remote code execution via HTTP
- Azure token provider abstraction and implementation
- Session management for code execution workflows
- File upload and download support
- Extensible dependency injection for easy integration
- Strongly-typed models for requests and results

## Installation

Install via NuGet:

```bash
dotnet add package Moedim.ACA.Sessions
```

Or via the NuGet Package Manager:

```powershell
PM> Install-Package Moedim.ACA.Sessions
```

## Usage Example

```csharp
using Moedim.ACA.Sessions;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddACASessions();

var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<ISessionsHttpClient>();

// Example: Execute code remotely
var result = await client.ExecuteCodeAsync(new CodeExecutionRequest {
 Code = "print('Hello, ACA!')",
 Language = "python"
});
Console.WriteLine(result.Output);
```

## Documentation

- [API Reference](../Moedim.ACA.Sessions/)
- [Example Project](../../Moedim.ACA.Sessions.Example/)

## License

This project is licensed under the MIT License. See the [LICENSE](../../LICENSE) file for details.

## Contributing

Contributions are welcome! Please open issues or submit pull requests via GitHub.
