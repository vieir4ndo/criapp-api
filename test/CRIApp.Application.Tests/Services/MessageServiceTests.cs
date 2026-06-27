using CRIApp.Application.Abstractions;
using CRIApp.Application.Resources;
using CRIApp.Application.Services;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Services;

public class MessageServiceTests
{
    private readonly Mock<ISmsClient> _smsClient = new();
    private readonly Mock<IEmailClient> _emailClient = new();
    private readonly Mock<ILogger<MessageService>> _logger = new();
    private readonly Mock<IStringLocalizer<MessageTemplates>> _emailLocalizerMock = new();

    public MessageServiceTests()
    {
        _emailLocalizerMock
            .Setup(l => l[It.IsAny<string>()])
            .Returns<string>(key => new LocalizedString(key, key));
    }

    private EmailTemplates CreateEmailTemplates() => new(_emailLocalizerMock.Object);

    private MessageService CreateSut(string senderPhone = "+34600000000", string senderEmail = "no-reply@criapp.com")
    {
        var twilioOptions = Options.Create(new TwilioSettings
        {
            ClientId = "AC123",
            ClientSecret = "secret",
            SenderPhoneNumber = senderPhone
        });
        var mailjetOptions = Options.Create(new MailjetSettings
        {
            BaseUrl = "https://api.mailjet.com",
            ApiKey = "key",
            ApiKeySecret = "keysecret",
            SenderEmail = senderEmail
        });
        return new MessageService(
            _smsClient.Object,
            _emailClient.Object,
            twilioOptions,
            mailjetOptions,
            _logger.Object,
            CreateEmailTemplates(),
            _emailLocalizerMock.Object);
    }

    private static User PhoneUser() => new("Parent", new DateTime(1990, 1, 1), Sex.Male, "+34600000001", "pass123");
    private static User EmailUser() => new("Parent", new DateTime(1990, 1, 1), Sex.Male, "user@criapp.com", "pass123");

    [Fact]
    public async Task NotifyAccountCreation_WithPhoneUser_ShouldCallSmsClient()
    {
        const string smsText = "Bienvenido a CRIApp";
        _emailLocalizerMock
            .Setup(l => l["Sms_AccountCreation"])
            .Returns(new LocalizedString("Sms_AccountCreation", smsText));
        var sut = CreateSut();
        var user = PhoneUser();

        await sut.NotifyAccountCreation(user);

        _smsClient.Verify(s => s.SendAsync(user.Username, "+34600000000", smsText), Times.Once);
        _emailClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task NotifyAccountCreation_WithEmailUser_ShouldCallEmailClient()
    {
        const string subject = "¡Bienvenido/a a CRIApp!";
        _emailLocalizerMock
            .Setup(l => l["Email_AccountCreation_Subject"])
            .Returns(new LocalizedString("Email_AccountCreation_Subject", subject));
        var sut = CreateSut(senderEmail: "no-reply@criapp.com");
        var user = EmailUser();

        await sut.NotifyAccountCreation(user);

        _emailClient.Verify(e => e.SendAsync("no-reply@criapp.com", user.Username, subject, It.IsAny<string>()), Times.Once);
        _smsClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task NotifyAccountCreation_WhenSmsClientThrows_ShouldNotPropagateException()
    {
        _smsClient.Setup(s => s.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException());
        var sut = CreateSut();
        var user = PhoneUser();

        var exception = await Record.ExceptionAsync(() => sut.NotifyAccountCreation(user));

        Assert.Null(exception);
    }

    [Fact]
    public async Task NotifyAccountCreation_WhenEmailClientThrows_ShouldNotPropagateException()
    {
        _emailClient.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException());
        var sut = CreateSut();
        var user = EmailUser();

        var exception = await Record.ExceptionAsync(() => sut.NotifyAccountCreation(user));

        Assert.Null(exception);
    }

    [Fact]
    public async Task NotifyForgotPassword_WithPhoneUser_ShouldCallSmsClientWithCode()
    {
        _emailLocalizerMock
            .Setup(l => l["Sms_ForgotPassword"])
            .Returns(new LocalizedString("Sms_ForgotPassword", "Tu codigo de recuperacion es: {0}"));
        var sut = CreateSut();
        var user = PhoneUser();

        await sut.NotifyForgotPassword(user, "AB123");

        _smsClient.Verify(s => s.SendAsync(user.Username, "+34600000000", "Tu codigo de recuperacion es: AB123"), Times.Once);
        _emailClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task NotifyForgotPassword_WithEmailUser_ShouldCallEmailClientWithCode()
    {
        const string subject = "Recuperación de contraseña — CRIApp";
        _emailLocalizerMock
            .Setup(l => l["Email_ForgotPassword_Subject"])
            .Returns(new LocalizedString("Email_ForgotPassword_Subject", subject));
        var sut = CreateSut(senderEmail: "no-reply@criapp.com");
        var user = EmailUser();

        await sut.NotifyForgotPassword(user, "XY999");

        _emailClient.Verify(e => e.SendAsync("no-reply@criapp.com", user.Username, subject, It.Is<string>(b => b.Contains("XY999"))), Times.Once);
        _smsClient.VerifyNoOtherCalls();
    }
}
