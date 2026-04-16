using CRIApp.Application.Abstractions;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRIApp.Application.Services;

public class MessageService : IMessageUser
{
    private readonly ISmsClient _smsClient;
    private readonly IEmailClient _emailClient;
    private readonly TwilioSettings _twilioSettings;
    private readonly MailjetSettings _mailjetSettings;
    private readonly ILogger<MessageService> _logger;

    public MessageService(
        ISmsClient smsClient,
        IEmailClient emailClient,
        IOptions<TwilioSettings> twilioSettings,
        IOptions<MailjetSettings> mailjetSettings,
        ILogger<MessageService> logger)
    {
        _smsClient = smsClient;
        _emailClient = emailClient;
        _twilioSettings = twilioSettings.Value;
        _mailjetSettings = mailjetSettings.Value;
        _logger = logger;
    }

    public async Task NotifyAccountCreation(User user)
    {
        try
        {
            var message = "Bienvenido a CRIApp";
            if (user.UsernameType() == UsernameType.PhoneNumber)
                _smsClient.Send(user.Username, _twilioSettings.SenderPhoneNumber, message);
            else
                await _emailClient.SendAsync(_mailjetSettings.SenderEmail, user.Username, message, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public async Task NotifyForgotPassword(User user, string code)
    {
        var message = "Tu codigo de recuperacion es: " + code;
        if (user.UsernameType() == UsernameType.PhoneNumber)
            _smsClient.Send(user.Username, _twilioSettings.SenderPhoneNumber, message);
        else
            await _emailClient.SendAsync(_mailjetSettings.SenderEmail, user.Username, message, message);
    }
}