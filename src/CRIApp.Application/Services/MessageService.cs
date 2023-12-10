using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using CRIApp.Application.Abstractions;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace CRIApp.Application.Services;

public class MessageService : IMessageUser
{
    private readonly TwilioSettings _twilioSettings;
    private readonly MailjetSettings _mailjetSettings;
    public MessageService(IOptions<TwilioSettings> messageSettings, IOptions<MailjetSettings> mailjetSettings)
    {
        _twilioSettings = messageSettings.Value;
        _mailjetSettings = mailjetSettings.Value;
    }

    public async Task NotifyAccountCreation(User user)
    {
        var message = "Bienvenido a CRIApp";
        if (user.UsernameType() == UsernameType.PhoneNumber)
        {
            SendSms(user.Username, message);
        }
        else
        {
            await SendEmail(user.Username, message, message);
        }
    }

    private void SendSms(string phoneNumber, string message)
    {
        TwilioClient.Init(_twilioSettings.ClientId, _twilioSettings.ClientSecret);

        _ = MessageResource.Create(
            body: message,
            from: new Twilio.Types.PhoneNumber(_twilioSettings.SenderPhoneNumber),
            to: new Twilio.Types.PhoneNumber(phoneNumber)
        );
    }

    private async Task SendEmail(string email, string subject, string textBody)
    {
        var client = new SmtpClient();
        client.Host = _mailjetSettings.Host;
        client.Credentials = new NetworkCredential(_mailjetSettings.ApiKey, _mailjetSettings.ApiKeySecret);
        client.Port = _mailjetSettings.Port;

        var message = new MailMessage(_mailjetSettings.SenderEmail, email);
        message.Body = textBody;
        message.Subject = subject;

        await client.SendMailAsync(message);
    }
}