namespace Moedim.ACA.Sessions.Options;

/// <summary>
/// Options for the session code interpreter.
/// </summary>
/// <remarks>Registered as singleton.</remarks>
public sealed class SessionCodeInterpreterOptions
{
    /// <summary>
    /// The endpoint for the Python execution service.
    /// When setting this property the value is normalized into a base endpoint:
    /// - Removes "/python/execute" when present.
    /// - Ensures a trailing slash.
    /// Reading this property returns the normalized base endpoint.
    /// </summary>
    public Uri? Endpoint
    {
        get;
        set => field = value is null ? null : GetBaseEndpoint(value);
    }

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