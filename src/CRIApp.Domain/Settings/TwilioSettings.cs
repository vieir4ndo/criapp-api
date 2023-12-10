namespace CRIApp.Domain.Settings;

public class TwilioSettings
{
    public static string SectionName { get; private set;  }= nameof(TwilioSettings);
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string SenderPhoneNumber { get; set; }
}