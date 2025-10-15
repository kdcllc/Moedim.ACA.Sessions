namespace Moedim.ACA.Sessions;

internal interface ISessionsHttpClient
{
    Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, string sessionId, CancellationToken cancellationToken, HttpContent? httpContent = null);
}