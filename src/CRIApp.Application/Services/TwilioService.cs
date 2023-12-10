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

public class TwilioService : IMessageUser
{
    private readonly MessageSettings _settings;
    public TwilioService(IOptions<MessageSettings> messageSettings)
    {
        _settings = messageSettings.Value;
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
            await SendEmail(user.Username, user.Name, message, message);
        }
    }

    private void SendSms(string phoneNumber, string message)
    {
        TwilioClient.Init(_settings.ClientId, _settings.ClientSecret);

        _ = MessageResource.Create(
            body: message,
            from: new Twilio.Types.PhoneNumber(_settings.SenderPhoneNumber),
            to: new Twilio.Types.PhoneNumber(phoneNumber)
        );
    }

    private async Task SendEmail(string email, string name, string subject, string textBody)
    {
        using var message = new MailMessage();
        message.From = new MailAddress(
           _settings.SenderEmail,
           _settings.SenderEmailName
        );
        message.To.Add(new MailAddress(
           email,
           name
        ));
        message.Subject = subject;

        message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(
            textBody, null, MediaTypeNames.Text.Plain)
        );
        
        using var client = new SmtpClient(host: "smtp.sendgrid.net", port: 587);
        client.Credentials = new NetworkCredential(
            userName: _settings.ClientId,
            password: _settings.ClientSecret
        );

        await client.SendMailAsync(message);
    }
}