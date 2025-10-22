namespace Moedim.ACA.Sessions.Options;

/// <summary>
/// Azure Microsoft Entra Provider.
/// </summary>
public class AzureTokenProviderOptions
{
    /// <summary>
    /// Number of minutes to refresh the token before it expires.
    /// </summary>
    public required int RefreshBeforeMinutes { get; set; }
}