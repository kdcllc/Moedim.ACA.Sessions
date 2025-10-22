using Azure.Core;

namespace Moedim.ACA.Sessions;

/// <summary>
/// Provides methods to obtain and manage Azure Entra authentication tokens.
/// </summary>
public interface IAzureTokenProvider : IDisposable
{
    /// <summary>
    /// Clears any cached Azure Entra authentication tokens.
    /// </summary>
    void ClearCache();

    /// <summary>
    /// Asynchronously obtains an Azure Entra authentication token for the specified scopes.
    /// </summary>
    /// <param name="scopes">The scopes to request the token for. If not provided, uses default scopes.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the AccessToken.</returns>
    Task<AccessToken> GetTokenAsync(string[] scopes, CancellationToken cancellationToken);
}