# Moedim.ACA.Sessions

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/kdcllc/Moedim.ACA.Sessions/master/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Moedim.ACA.Sessions.svg)](https://www.nuget.org/packages?q=Moedim.ACA.Sessions)
![Nuget](https://img.shields.io/nuget/dt/Moedim.ACA.Sessions)

![Stand With Israel](./img/IStandWithIsrael.png)

> The second letter in the Hebrew alphabet is the ב bet/beit. Its meaning is "house". In the ancient pictographic Hebrew it was a symbol resembling a tent on a landscape.

**A C# library for Azure Container Apps dynamic sessions - enabling secure, isolated code execution for AI agents and applications.**

## 🚀 Overview

Moedim.ACA.Sessions provides a robust C# wrapper around Azure Container Apps' serverless code interpreter sessions, designed specifically for integration with **Agentic AI frameworks** where AI agents need to execute code safely and securely. This library enables AI agents to:

- Execute Python code in isolated, sandboxed environments
- Create and manipulate files (including PowerPoint presentations, Excel files, charts, etc.)
- Process data and generate visualizations
- Install and use popular Python packages (NumPy, pandas, matplotlib, etc.)
- Maintain session state across multiple code executions

## Hire me

Please send [email](mailto:kingdavidconsulting@gmail.com) if you consider to **hire me**.

[![buymeacoffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/vyve0og)

## Give a Star! :star:

If you like or are using this project to learn or start your solution, please give it a star. Thanks!

## Install

```bash
    dotnet add package Moedim.ACA.Sessions
```

## Projects

- [`Moedim.ACA.Sessions`](./src/Moedim.ACA.Sessions) - actual library
- [`Moedim.ACA.Sessions.Example`](./src/Moedim.ACA.Sessions.Example/) - usage

## Configuration

appsettings.json

```json
{
  "ACASessions": {
    "AzureTokenProvider": {
      "RefreshBeforeMinutes": 5,
      "Scopes": [
        "https://dynamicsessions.io/.default"
      ]
    },
    "SessionsHttpClient": {
      "Endpoint": "https://{region}.dynamicsessions.io/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/sessionPools/{sessionPoolName}",
      "ApiVersion": "2024-02-02-preview"
    }
  }
}
```

## 🔐 Authentication

Azure RBAC Requirements
Your application needs the following roles on the session pool:

- Azure ContainerApps Session Executor
- Contributor

Using Azure CLI (Development)

```bash
az account get-access-token --resource https://dynamicsessions.io
```

## 💻 Usage Examples

Basic Code Execution

```csharp

// Register all ACA Sessions services
services.AddACASessions();

public class DataAnalysisAgent
{
    private readonly ICodeInterpreter _codeInterpreter;

    public DataAnalysisAgent(ICodeInterpreter codeInterpreter)
    {
        _codeInterpreter = codeInterpreter;
    }

    public async Task<string> AnalyzeDataAsync(string csvData, string sessionId)
    {
        // Upload CSV data
        await _codeInterpreter.UploadFileAsync(new FileUploadRequest
        {
            SessionId = sessionId,
            FileName = "data.csv",
            FileContent = Encoding.UTF8.GetBytes(csvData)
        }, CancellationToken.None);

        // Analyze data and create visualization
        var analysisCode = """
            import pandas as pd
            import matplotlib.pyplot as plt
            import json

            # Load and analyze data
            df = pd.read_csv('/mnt/data/data.csv')

            # Generate summary statistics
            summary = {
                'total_rows': len(df),
                'columns': list(df.columns),
                'numeric_summary': df.describe().to_dict()
            }

            # Create visualization
            plt.figure(figsize=(10, 6))
            df.hist(bins=20, figsize=(12, 8))
            plt.suptitle('Data Distribution')
            plt.tight_layout()
            plt.savefig('/mnt/data/analysis_chart.png', dpi=300, bbox_inches='tight')

            # Save analysis results
            with open('/mnt/data/analysis_results.json', 'w') as f:
                json.dump(summary, f, indent=2)

            print("Analysis complete!")
            print(f"Dataset contains {len(df)} rows and {len(df.columns)} columns")
            """;

        var result = await _codeInterpreter.ExecuteAsync(new CodeExecutionRequest
        {
            SessionId = sessionId,
            Code = analysisCode,
            SanitizeInput = true
        }, CancellationToken.None);

        return result.ToString();
    }
}
```

## References

- [Serverless code interpreter sessions in Azure Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/sessions-code-interpreter)
