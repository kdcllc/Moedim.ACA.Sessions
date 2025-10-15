using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moedim.ACA.Sessions.Options;

namespace Moedim.ACA.Sessions;

/// <summary>
/// Encapsulates HTTP client logic for communicating with a Sessions service.
/// Extracted from code interpreter logic so HTTP concerns are isolated.
/// </summary>
internal sealed class SessionsHttpClient : ISessionsHttpClient
{
    private readonly ILogger<SessionsHttpClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly Uri _baseEndpoint;
    private readonly IAzureTokenProvider? _authTokenProvider;
    private readonly string _apiVersion;
    private readonly string _userAgent;

    public SessionsHttpClient(
        HttpClient httpClient,
        IOptions<SessionsHttpClientOptions> options,
        IAzureTokenProvider azureTokenProvider,
        ILogger<SessionsHttpClient> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options.Value);
        ArgumentNullException.ThrowIfNull(azureTokenProvider);
        ArgumentNullException.ThrowIfNull(logger);

        if (string.IsNullOrWhiteSpace(options.Value.ApiVersion))
        {
            throw new ArgumentException("ApiVersion cannot be null or empty.", nameof(options));
        }

        _apiVersion = options.Value.ApiVersion!;

        // Prefer a base endpoint from the settings, fall back to the options' endpoint.
        if (options.Value.Endpoint is null)
        {
            throw new ArgumentException("Endpoint must be provided in either settings or options.", nameof(options));
        }

        _httpClient = httpClient;
        _baseEndpoint = options.Value.Endpoint!;

        _authTokenProvider = azureTokenProvider;
        _logger = logger;
        _userAgent = $"Moedim.ACA.Sessions/{typeof(SessionsHttpClient).Assembly.GetName().Version?.ToString() ?? "1.0.0"} (Language=dotnet)";
    }

    /// <summary>
    /// Sends an HTTP request to the specified path and verifies the response is successful.
    /// </summary>
    public async Task<HttpResponseMessage> SendAsync(
        HttpMethod method,
        string path,
        string sessionId,
        CancellationToken cancellationToken,
        HttpContent? httpContent = null)
    {
        var pathWithQueryString = $"{path}?identifier={sessionId}&api-version={_apiVersion}";

        var uri = new Uri(_baseEndpoint, pathWithQueryString);

        using var request = new HttpRequestMessage(method, uri)
        {
            Content = httpContent,
        };

        await AddHeadersAsync(request, cancellationToken).ConfigureAwait(false);

        HttpResponseMessage? response = null;

        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            _logger.LogDebug("Sending {Method} request to {Uri}", method, uri);
        }
        catch (HttpRequestException e)
        {
            throw new HttpRequestException($"Error sending request to {uri}: {e.Message}", e);
        }

        if (!response.IsSuccessStatusCode)
        {
            string? responseContent = null;
            try
            {
                // On .NET Framework, EnsureSuccessStatusCode disposes of the response content;
                // that was changed years ago in .NET Core, but for .NET Framework it means in order
                // to read the response content in the case of failure, that has to be
                // done before calling EnsureSuccessStatusCode.
                responseContent = await response!.Content.ReadAsStringAsync().ConfigureAwait(false);
                response.EnsureSuccessStatusCode(); // will always throw
            }
            catch (Exception e)
            {
                throw new HttpRequestException($"Request to {uri} failed with status code {(int)response.StatusCode}: {response.ReasonPhrase}. Response: {responseContent}", e);
            }
        }

        return response;
    }

    /// <summary>
    /// Add required headers to the request instance.
    /// </summary>
    private async Task AddHeadersAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // User-Agent
        request.Headers.TryAddWithoutValidation("User-Agent", _userAgent);

        // Optional auth header
        if (_authTokenProvider is not null)
        {
            var token = await _authTokenProvider.GetTokenAsync(cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
