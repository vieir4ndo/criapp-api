using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Api.Tests.Requests;

public class ResetPasswordRequestTests
{
    private static ResetPasswordRequest ValidRequest() => new ResetPasswordRequest
    {
        Username = "user@domain.com",
        Code = "ABC12",
        NewPassword = "newpassword"
    };

    [Fact]
    public void ValidRequest_ShouldNotThrow()
    {
        var ex = Record.Exception(() => ValidRequest().ValidateAndThrow());

        Assert.Null(ex);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void EmptyOrNullUsername_ShouldThrowValidationException(string? username)
    {
        var request = ValidRequest();
        request.Username = username!;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Theory]
    [InlineData("not-valid")]
    [InlineData("plaintext")]
    public void InvalidUsername_ShouldThrowValidationException(string username)
    {
        var request = ValidRequest();
        request.Username = username;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void EmptyOrNullCode_ShouldThrowValidationException(string? code)
    {
        var request = ValidRequest();
        request.Code = code!;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }
}
