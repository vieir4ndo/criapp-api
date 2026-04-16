using System.Diagnostics.CodeAnalysis;
using CRIApp.Application.Abstractions;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace CRIApp.Application.Services;

[ExcludeFromCodeCoverage]
public class TwilioSmsClient : ISmsClient
{
    private readonly TwilioSettings _settings;

    public TwilioSmsClient(IOptions<TwilioSettings> settings)
    {
        _settings = settings.Value;
        TwilioClient.Init(_settings.ClientId, _settings.ClientSecret);
    }

    public void Send(string to, string from, string body)
    {
        MessageResource.Create(
            body: body,
            from: new Twilio.Types.PhoneNumber(from),
            to: new Twilio.Types.PhoneNumber(to)
        );
    }
}
