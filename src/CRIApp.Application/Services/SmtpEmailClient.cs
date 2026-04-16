using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mail;
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
        message.Body = body;
        message.Subject = subject;

        await client.SendMailAsync(message);
    }
}
