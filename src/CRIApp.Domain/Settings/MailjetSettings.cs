using System.Diagnostics.CodeAnalysis;

namespace CRIApp.Domain.Settings;

[ExcludeFromCodeCoverage]
public class MailjetSettings
{
    public static string SectionName { get; private set;  }= nameof(MailjetSettings);
    public string BaseUrl { get; set; } = "https://api.mailjet.com";
    public string ApiKey { get; set; } = null!;
    public string ApiKeySecret { get; set; } = null!;
    public string SenderEmail { get; set; } = null!;
}