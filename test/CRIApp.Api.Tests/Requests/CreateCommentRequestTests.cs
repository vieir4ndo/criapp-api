using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Api.Tests.Requests;

public class CreateCommentRequestTests
{
    private static CreateCommentRequest ValidRequest() => new CreateCommentRequest
    {
        Text = "A comment",
        ForumId = Guid.NewGuid()
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
    public void EmptyOrNullText_ShouldThrowValidationException(string? text)
    {
        var request = ValidRequest();
        request.Text = text!;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Fact]
    public void EmptyForumId_ShouldThrowValidationException()
    {
        var request = ValidRequest();
        request.ForumId = Guid.Empty;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }
}
