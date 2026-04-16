using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class QuestionTests
{
    private static readonly Guid ValidAdminId = Guid.NewGuid();

    private static Question CreateValidQuestion(string text = "Does your child sleep well?")
        => new Question(text, ValidAdminId, QuestionType.Agreement);

    [Fact]
    public void Create_ValidQuestion_ShouldSucceed()
    {
        var question = CreateValidQuestion();

        Assert.NotNull(question);
        Assert.Equal("Does your child sleep well?", question.Text);
        Assert.Equal(ValidAdminId, question.AdminId);
        Assert.Equal(QuestionType.Agreement, question.Type);
        Assert.True(question.Active);
    }

    [Fact]
    public void Create_WithEmptyText_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Question("", ValidAdminId, QuestionType.Agreement));
    }

    [Fact]
    public void Create_WithNullText_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Question(null!, ValidAdminId, QuestionType.Agreement));
    }

    [Fact]
    public void Create_WithEmptyAdminId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() =>
            new Question("Valid text?", Guid.Empty, QuestionType.Agreement));
    }

    [Fact]
    public void Deactivate_ShouldSetActiveToFalse()
    {
        var question = CreateValidQuestion();

        question.Deactivate();

        Assert.False(question.Active);
    }

    [Fact]
    public void Activate_AfterDeactivate_ShouldSetActiveToTrue()
    {
        var question = CreateValidQuestion();
        question.Deactivate();

        question.Activate();

        Assert.True(question.Active);
    }

    [Fact]
    public void ChangeText_WithValidText_ShouldUpdateText()
    {
        var question = CreateValidQuestion();

        question.ChangeText("How often does your child eat vegetables?");

        Assert.Equal("How often does your child eat vegetables?", question.Text);
    }

    [Fact]
    public void ChangeText_WithEmptyText_ShouldThrowDomainException()
    {
        var question = CreateValidQuestion();

        Assert.Throws<DomainException>(() => question.ChangeText(""));
    }

    [Fact]
    public void Create_WithFrequencyType_ShouldSetTypeCorrectly()
    {
        var question = new Question("How often?", ValidAdminId, QuestionType.Frequency);

        Assert.Equal(QuestionType.Frequency, question.Type);
    }
}
