namespace CRIApp.Domain.Settings;

public class MessageSettings
{
    public static string SectionName { get; private set;  }= nameof(MessageSettings);
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string SenderPhoneNumber { get; set; }
    public string SenderEmail { get; set; }
}