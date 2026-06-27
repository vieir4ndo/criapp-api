using System.Diagnostics.CodeAnalysis;

namespace CRIApp.Domain.Settings;

[ExcludeFromCodeCoverage]
public class TwilioSettings
{
    public static string SectionName { get; private set;  }= nameof(TwilioSettings);
    public string BaseUrl { get; set; } = "https://api.twilio.com";
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string SenderPhoneNumber { get; set; } = null!;
}