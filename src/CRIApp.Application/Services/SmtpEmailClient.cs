using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using CRIApp.Application.Abstractions;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Options;

namespace CRIApp.Application.Services;

[ExcludeFromCodeCoverage]
public class SmtpEmailClient : IEmailClient
{
    private readonly MailjetSettings _settings;

    public SmtpEmailClient(IOptions<MailjetSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendAsync(string from, string to, string subject, string body)
    {
        using var client = new SmtpClient();
        client.Host = _settings.Host;
        client.Credentials = new NetworkCredential(_settings.ApiKey, _settings.ApiKeySecret);
        client.Port = _settings.Port;

        var message = new MailMessage(from, to);
        message.Subject = subject;

        var htmlView = AlternateView.CreateAlternateViewFromString(body, null, MediaTypeNames.Text.Html);
        var logoBytes = Convert.FromBase64String(EmailTemplates.LogoBase64);
        var logoResource = new LinkedResource(new MemoryStream(logoBytes), "image/png")
        {
            ContentId = EmailTemplates.LogoContentId,
            TransferEncoding = TransferEncoding.Base64
        };
        htmlView.LinkedResources.Add(logoResource);
        message.AlternateViews.Add(htmlView);

        await client.SendMailAsync(message);
    }
}
