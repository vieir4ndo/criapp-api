using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Api.Tests.Requests;

public class CreateForumRequestTests
{
    private static CreateForumRequest ValidRequest() => new CreateForumRequest
    {
        Name = "My Forum",
        Description = "A forum description"
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
    public void EmptyOrNullName_ShouldThrowValidationException(string? name)
    {
        var request = ValidRequest();
        request.Name = name!;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void EmptyOrNullDescription_ShouldThrowValidationException(string? description)
    {
        var request = ValidRequest();
        request.Description = description!;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }
}
