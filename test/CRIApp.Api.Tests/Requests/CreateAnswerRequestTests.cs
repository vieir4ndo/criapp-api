using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Api.Tests.Requests;

public class CreateAnswerRequestTests
{
    private static CreateAnswerRequest ValidRequest() => new CreateAnswerRequest
    {
        Value = 3,
        QuestionId = Guid.NewGuid(),
        ChildId = Guid.NewGuid()
    };

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void ValidValue_ShouldNotThrow(int value)
    {
        var request = ValidRequest();
        request.Value = value;

        var ex = Record.Exception(() => request.ValidateAndThrow());

        Assert.Null(ex);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    public void ValueOutOfRange_ShouldThrowValidationException(int value)
    {
        var request = ValidRequest();
        request.Value = value;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Fact]
    public void EmptyQuestionId_ShouldThrowValidationException()
    {
        var request = ValidRequest();
        request.QuestionId = Guid.Empty;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Fact]
    public void EmptyChildId_ShouldThrowValidationException()
    {
        var request = ValidRequest();
        request.ChildId = Guid.Empty;

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }
}
