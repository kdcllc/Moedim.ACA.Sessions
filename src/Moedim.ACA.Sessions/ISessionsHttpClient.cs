namespace Moedim.ACA.Sessions;

/// <summary>
/// Encapsulates HTTP client logic for communicating with a Sessions service.
/// Extracted from code interpreter logic so HTTP concerns are isolated.
/// </summary>
public interface ISessionsHttpClient
{
    /// <summary>
    /// Sends an HTTP request to the specified path and verifies the response is successful.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="path"></param>
    /// <param name="sessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="httpContent"></param>
    /// <returns></returns>
    Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string path,
        string sessionId,
        CancellationToken cancellationToken,
        HttpContent? httpContent = null);
}