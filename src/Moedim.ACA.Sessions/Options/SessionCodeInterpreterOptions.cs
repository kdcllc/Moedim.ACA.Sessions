namespace Moedim.ACA.Sessions.Options;

/// <summary>
/// Options for the session code interpreter.
/// </summary>
/// <remarks>Registered as singleton.</remarks>
public sealed class SessionCodeInterpreterOptions
{
    /// <summary>
    /// The endpoint for the Python execution service.
    /// </summary>
    public Uri? Endpoint { get; set; }

    /// <summary>
    /// The computed base endpoint for the Python execution service.
    /// Returns null when <see cref="Endpoint"/> is not set.
    /// </summary>
    public Uri? BaseEndpoint => Endpoint is null ? null : GetBaseEndpoint(Endpoint);

    /// <summary>
    /// The API version to use when communicating with the Python execution service.
    /// </summary>
    public string ApiVersion { get; set; } = "2024-10-02-preview";

    /// <summary>
    /// Get the base endpoint for the Python execution service.
    /// </summary>
    /// <param name="endpoint">The full endpoint URI.</param>
    /// <returns>The base endpoint URI.</returns>
    private static Uri GetBaseEndpoint(Uri endpoint)
    {
        if (endpoint.PathAndQuery.Contains("/python/execute"))
        {
            endpoint = new Uri(endpoint.ToString().Replace("/python/execute", string.Empty));
        }

        if (!endpoint.PathAndQuery.EndsWith('/'))
        {
            endpoint = new Uri(endpoint + "/");
        }

        return endpoint;
    }
}