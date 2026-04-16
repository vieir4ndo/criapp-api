using System.Diagnostics.CodeAnalysis;

namespace CRIApp.Domain.Settings;

[ExcludeFromCodeCoverage]
public class MailjetSettings
{
    public static string SectionName { get; private set;  }= nameof(MailjetSettings);
    public string Host { get; set; }
    public string ApiKey { get; set; }
    public string ApiKeySecret { get; set; }
    public int Port { get; set; }
    public string SenderEmail { get; set; }
}