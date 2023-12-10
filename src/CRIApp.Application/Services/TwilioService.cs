using CRIApp.Application.Abstractions;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace CRIApp.Application.Services;

public class TwilioService : IMessage
{
    private readonly MessageSettings _settings;
    public TwilioService(IOptions<MessageSettings> messageSettings)
    {
        _settings = messageSettings.Value;
    }
    public Task SendEmail(string email, string message)
    {
        throw new NotImplementedException();
    }

    public void SendSms(string phoneNumber, string message)
    {
        TwilioClient.Init(_settings.ClientId, _settings.ClientSecret);

        _ = MessageResource.Create(
            body: message,
            from: new Twilio.Types.PhoneNumber(_settings.SenderPhoneNumber),
            to: new Twilio.Types.PhoneNumber(phoneNumber)
        );
    }
}