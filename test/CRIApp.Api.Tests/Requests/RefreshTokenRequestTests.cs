using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Api.Tests.Requests;

public class RefreshTokenRequestTests
{
    private static RefreshTokenRequest ValidRequest() => new RefreshTokenRequest
    {
        RefreshToken = "valid-refresh-token"
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
    [InlineData("   ")]
    public void Request_NullOrEmptyToken_ShouldThrowValidationException(string? token)
    {
        var request = new RefreshTokenRequest { RefreshToken = token! };

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }
}
