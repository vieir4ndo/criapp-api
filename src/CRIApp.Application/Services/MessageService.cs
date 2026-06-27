using CRIApp.Application.Abstractions;
using CRIApp.Application.Resources;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Localization;
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
    private readonly EmailTemplates _emailTemplates;
    private readonly IStringLocalizer<MessageTemplates> _localizer;

    public MessageService(
        ISmsClient smsClient,
        IEmailClient emailClient,
        IOptions<TwilioSettings> twilioSettings,
        IOptions<MailjetSettings> mailjetSettings,
        ILogger<MessageService> logger,
        EmailTemplates emailTemplates,
        IStringLocalizer<MessageTemplates> localizer)
    {
        _smsClient = smsClient;
        _emailClient = emailClient;
        _twilioSettings = twilioSettings.Value;
        _mailjetSettings = mailjetSettings.Value;
        _logger = logger;
        _emailTemplates = emailTemplates;
        _localizer = localizer;
    }

    public async Task NotifyAccountCreation(User user)
    {
        try
        {
            if (user.UsernameType() == UsernameType.PhoneNumber)
                await _smsClient.SendAsync(user.Username, _twilioSettings.SenderPhoneNumber, _localizer["Sms_AccountCreation"]);
            else
                await _emailClient.SendAsync(
                    _mailjetSettings.SenderEmail,
                    user.Username,
                    _localizer["Email_AccountCreation_Subject"],
                    _emailTemplates.AccountCreation(user.Username));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public async Task NotifyForgotPassword(User user, string code)
    {
        if (user.UsernameType() == UsernameType.PhoneNumber)
            await _smsClient.SendAsync(user.Username, _twilioSettings.SenderPhoneNumber,
                string.Format(_localizer["Sms_ForgotPassword"], code));
        else
            await _emailClient.SendAsync(
                _mailjetSettings.SenderEmail,
                user.Username,
                _localizer["Email_ForgotPassword_Subject"],
                _emailTemplates.ForgotPassword(code));
    }
}