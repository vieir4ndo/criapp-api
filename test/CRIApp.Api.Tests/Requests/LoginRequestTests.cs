using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Api.Tests.Requests;

public class LoginRequestTests
{
    [Fact]
    public void ValidRequest_WithEmail_ShouldNotThrow()
    {
        var request = new LoginRequest { Username = "user@domain.com", Password = "pass123" };

        var ex = Record.Exception(() => request.ValidateAndThrow());

        Assert.Null(ex);
    }

    [Fact]
    public void ValidRequest_WithPhone_ShouldNotThrow()
    {
        var request = new LoginRequest { Username = "+34612345678", Password = "pass123" };

        var ex = Record.Exception(() => request.ValidateAndThrow());

        Assert.Null(ex);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void EmptyOrNullUsername_ShouldThrowValidationException(string? username)
    {
        var request = new LoginRequest { Username = username!, Password = "pass123" };

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Theory]
    [InlineData("not-an-email-or-phone")]
    [InlineData("invalid_user")]
    public void InvalidUsername_ShouldThrowValidationException(string username)
    {
        var request = new LoginRequest { Username = username, Password = "pass123" };

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void EmptyOrNullPassword_ShouldThrowValidationException(string? password)
    {
        var request = new LoginRequest { Username = "user@domain.com", Password = password! };

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }
}
