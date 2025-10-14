namespace Moedim.ACA.Sessions;

/// <summary>
/// Provides methods to obtain and manage Azure authentication tokens.
/// </summary>
public interface IAzureTokenProvider : IDisposable
{
    /// <summary>
    /// Clears any cached Azure authentication tokens.
    /// </summary>
    void ClearCache();

    /// <summary>
    /// Asynchronously obtains an Azure authentication token.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the authentication token as a string.</returns>
    Task<string> GetTokenAsync(CancellationToken cancellationToken);
}