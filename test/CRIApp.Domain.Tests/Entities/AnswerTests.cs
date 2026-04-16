using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class AnswerTests
{
    private static readonly Guid ValidQuestionId = Guid.NewGuid();
    private static readonly Guid ValidChildId = Guid.NewGuid();
    private static readonly Guid ValidPrcId = Guid.NewGuid();

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Create_WithValidValue_ShouldSucceed(int value)
    {
        var answer = new Answer(value, ValidQuestionId, ValidChildId, ValidPrcId);

        Assert.NotNull(answer);
        Assert.Equal(value, answer.Value);
        Assert.Equal(ValidQuestionId, answer.QuestionId);
        Assert.Equal(ValidChildId, answer.ChildId);
        Assert.Equal(ValidPrcId, answer.PrcId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(10)]
    public void Create_WithOutOfRangeValue_ShouldThrowDomainException(int value)
    {
        Assert.Throws<DomainException>(() =>
            new Answer(value, ValidQuestionId, ValidChildId, ValidPrcId));
    }

    [Fact]
    public void Create_WithEmptyQuestionId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Answer(3, Guid.Empty, ValidChildId, ValidPrcId));
    }

    [Fact]
    public void Create_WithEmptyPrcId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Answer(3, ValidQuestionId, ValidChildId, Guid.Empty));
    }

    [Fact]
    public void Create_ShouldSetCreatedAtToUtcNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);

        var answer = new Answer(3, ValidQuestionId, ValidChildId, ValidPrcId);

        Assert.True(answer.CreatedAt >= before);
        Assert.True(answer.CreatedAt <= DateTime.UtcNow.AddSeconds(1));
    }
}
