# Moedim.ACA.Sessions

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/kdcllc/Moedim.ACA.Sessions/master/LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Moedim.ACA.Sessions.svg)](https://www.nuget.org/packages?q=Moedim.ACA.Sessions)
![Nuget](https://img.shields.io/nuget/dt/Moedim.ACA.Sessions)

![Stand With Israel](./img/IStandWithIsrael.png)

> The second letter in the Hebrew alphabet is the ב bet/beit. Its meaning is "house". In the ancient pictographic Hebrew it was a symbol resembling a tent on a landscape.

Library for dynamic sessions in Azure Container Apps

## Hire me

Please send [email](mailto:kingdavidconsulting@gmail.com) if you consider to **hire me**.

[![buymeacoffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/vyve0og)

## Give a Star! :star:

If you like or are using this project to learn or start your solution, please give it a star. Thanks!


## Configuration

```json
  "ACASessions": {
    "AzureTokenProvider": {
      "RefreshBeforeMinutes": 5,
      "Scopes": [
        "https://dynamicsessions.io/.default"
      ]
    },
    "SessionsHttpClient": {
      "Endpoint": "http://localhost:5000/python/execute/",
      "ApiVersion": "v1"
    }
  }
```

## Authentication

```bash
az account get-access-token --resource https://dynamicsessions.io
```

## References

- [Serverless code interpreter sessions in Azure Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/sessions-code-interpreter)
