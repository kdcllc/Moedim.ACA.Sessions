# Moedim.ACA.Sessions

Library for dynamic sessions in Azure Container Apps

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
