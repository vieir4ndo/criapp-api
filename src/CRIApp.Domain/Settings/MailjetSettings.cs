using System.Diagnostics.CodeAnalysis;

namespace CRIApp.Domain.Settings;

[ExcludeFromCodeCoverage]
public class MailjetSettings
{
    public static string SectionName { get; private set;  }= nameof(MailjetSettings);
    public string Host { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string ApiKeySecret { get; set; } = null!;
    public int Port { get; set; }
    public string SenderEmail { get; set; } = null!;
}