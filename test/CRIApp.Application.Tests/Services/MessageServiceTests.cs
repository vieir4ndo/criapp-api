using CRIApp.Application.Abstractions;
using CRIApp.Application.Services;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Settings;
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
            Host = "smtp.mailjet.com",
            ApiKey = "key",
            ApiKeySecret = "keysecret",
            Port = 587,
            SenderEmail = senderEmail
        });
        return new MessageService(_smsClient.Object, _emailClient.Object, twilioOptions, mailjetOptions, _logger.Object);
    }

    private static User PhoneUser() => new("Parent", new DateTime(1990, 1, 1), Sex.Male, "+34600000001", "pass123");
    private static User EmailUser() => new("Parent", new DateTime(1990, 1, 1), Sex.Male, "user@criapp.com", "pass123");

    [Fact]
    public async Task NotifyAccountCreation_WithPhoneUser_ShouldCallSmsClient()
    {
        var sut = CreateSut();
        var user = PhoneUser();

        await sut.NotifyAccountCreation(user);

        _smsClient.Verify(s => s.Send(user.Username, "+34600000000", "Bienvenido a CRIApp"), Times.Once);
        _emailClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task NotifyAccountCreation_WithEmailUser_ShouldCallEmailClient()
    {
        var sut = CreateSut(senderEmail: "no-reply@criapp.com");
        var user = EmailUser();

        await sut.NotifyAccountCreation(user);

        _emailClient.Verify(e => e.SendAsync("no-reply@criapp.com", user.Username, "Bienvenido a CRIApp", "Bienvenido a CRIApp"), Times.Once);
        _smsClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task NotifyAccountCreation_WhenSmsClientThrows_ShouldNotPropagateException()
    {
        _smsClient.Setup(s => s.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Throws<InvalidOperationException>();
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
        var sut = CreateSut();
        var user = PhoneUser();

        await sut.NotifyForgotPassword(user, "AB123");

        _smsClient.Verify(s => s.Send(user.Username, "+34600000000", "Tu codigo de recuperacion es: AB123"), Times.Once);
        _emailClient.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task NotifyForgotPassword_WithEmailUser_ShouldCallEmailClientWithCode()
    {
        var sut = CreateSut(senderEmail: "no-reply@criapp.com");
        var user = EmailUser();

        await sut.NotifyForgotPassword(user, "XY999");

        _emailClient.Verify(e => e.SendAsync("no-reply@criapp.com", user.Username, "Tu codigo de recuperacion es: XY999", "Tu codigo de recuperacion es: XY999"), Times.Once);
        _smsClient.VerifyNoOtherCalls();
    }
}
