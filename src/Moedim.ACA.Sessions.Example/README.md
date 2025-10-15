
# Moedim.ACA.Sessions.Example

This is a sample console application demonstrating how to use the Moedim.ACA.Sessions library in a real-world scenario. It showcases best practices for dependency injection, configuration, and logging in .NET console apps.

## Features

- Custom `AppHost` for out-of-the-box Dependency Injection support
- Integrated `Serilog` logging from start to finish
- Single configuration for the lifetime of the application
- Example usage of Moedim.ACA.Sessions for remote code execution and session management

## Getting Started


1. Clone the repository and navigate to the example app directory:

```bash
cd src/Moedim.ACA.Sessions.Example
```

1. Restore dependencies:

```bash
dotnet restore
```

1. Run the example:

```bash
dotnet run
```

## How It Works

The example app configures dependency injection and logging, then demonstrates how to use the `ISessionsHttpClient` from Moedim.ACA.Sessions to execute code remotely and manage sessions. See `Program.cs` and `Main.cs` for implementation details.

## Related Resources

- [Moedim.ACA.Sessions Library](../Moedim.ACA.Sessions/README.md)
- [Bet.Extensions.Templating](https://github.com/kdcllc/Bet.Extensions.Templating)

## License

This project is licensed under the MIT License. See the [LICENSE](../../LICENSE) file for details.
