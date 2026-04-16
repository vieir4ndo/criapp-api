using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Api.Tests.Requests;

public class ForgotPasswordRequestTests
{
    [Fact]
    public void ValidEmail_ShouldNotThrow()
    {
        var request = new ForgotPasswordRequest { Username = "user@domain.com" };

        var ex = Record.Exception(() => request.ValidateAndThrow());

        Assert.Null(ex);
    }

    [Fact]
    public void ValidPhone_ShouldNotThrow()
    {
        var request = new ForgotPasswordRequest { Username = "+34612345678" };

        var ex = Record.Exception(() => request.ValidateAndThrow());

        Assert.Null(ex);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void EmptyOrNullUsername_ShouldThrowValidationException(string? username)
    {
        var request = new ForgotPasswordRequest { Username = username! };

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Theory]
    [InlineData("not-valid")]
    [InlineData("no_at_sign")]
    public void InvalidUsername_ShouldThrowValidationException(string username)
    {
        var request = new ForgotPasswordRequest { Username = username };

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }
}
