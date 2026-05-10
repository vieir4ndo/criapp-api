using CRIApp.Application.Jobs;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Reflection;
using Xunit;

namespace CRIApp.Application.Tests.Jobs;

public class SaveUsersAdvicesTests
{
    private readonly Mock<IChildBookRepository> _repositoryMock;
    private readonly SaveUsersAdvices _sut;

    private static readonly Guid AdminId = Guid.NewGuid();

    public SaveUsersAdvicesTests()
    {
        _repositoryMock = new Mock<IChildBookRepository>();
        _sut = new SaveUsersAdvices(NullLogger<SaveUsersAdvices>.Instance, _repositoryMock.Object);
    }

    private static ChildBook BuildChildBook()
    {
        var prcId = Guid.NewGuid();
        var child = new Child("Child", new DateTime(2020, 1, 1), Sex.Male, 6, prcId, false);
        typeof(Child).GetProperty(nameof(Child.Answers))!.SetValue(child, new List<Answer>());

        var childBook = new ChildBook(child.Id);
        typeof(ChildBook).GetProperty(nameof(ChildBook.Child))!.SetValue(childBook, child);
        return childBook;
    }

    private static (Answer answer, Question question, Advice advice) BuildAnswerWithAdvice(
        int value, QuestionScoringType scoringType)
    {
        var advice = new Advice("Do this.", AdminId, Guid.NewGuid());
        var question = new Question("Q?", AdminId, QuestionType.Agreement, scoringType);
        typeof(Question).GetProperty(nameof(Question.Advices))!
            .SetValue(question, new List<Advice> { advice });

        var answer = new Answer(value, question.Id, Guid.NewGuid(), Guid.NewGuid());
        typeof(Answer).GetProperty(nameof(Answer.Question))!.SetValue(answer, question);

        return (answer, question, advice);
    }

    [Fact]
    public async Task Run_NoChildBooks_ShouldNotCallUpdate()
    {
        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook>());

        await _sut.Run();

        _repositoryMock.Verify(r => r.Update(It.IsAny<ChildBook>()), Times.Never);
    }

    [Fact]
    public async Task Run_ChildBookWithNoAnswers_ShouldNotCallUpdate()
    {
        var childBook = BuildChildBook();
        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        _repositoryMock.Verify(r => r.Update(It.IsAny<ChildBook>()), Times.Never);
    }

    [Fact]
    public async Task Run_AnswerWithBadDefaultScore_ShouldAddAdvice()
    {
        var childBook = BuildChildBook();
        var (answer, _, advice) = BuildAnswerWithAdvice(5, QuestionScoringType.Default);
        childBook.Child.Answers.Add(answer);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.Contains(advice, childBook.Advices);
    }

    [Fact]
    public async Task Run_AnswerWithGoodDefaultScore_ShouldNotAddAdvice()
    {
        var childBook = BuildChildBook();
        var (answer, _, advice) = BuildAnswerWithAdvice(1, QuestionScoringType.Default);
        childBook.Child.Answers.Add(answer);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.DoesNotContain(advice, childBook.Advices);
    }

    [Fact]
    public async Task Run_AnswerWithBadReversedScore_ShouldAddAdvice()
    {
        var childBook = BuildChildBook();
        var (answer, _, advice) = BuildAnswerWithAdvice(1, QuestionScoringType.Reversed);
        childBook.Child.Answers.Add(answer);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.Contains(advice, childBook.Advices);
    }

    [Fact]
    public async Task Run_AnswerWithGoodDefaultScore_ShouldNotCallUpdate()
    {
        var childBook = BuildChildBook();
        var (answer, _, _) = BuildAnswerWithAdvice(1, QuestionScoringType.Default);
        childBook.Child.Answers.Add(answer);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        _repositoryMock.Verify(r => r.Update(It.IsAny<ChildBook>()), Times.Never);
    }

    [Fact]
    public async Task Run_AnswerWithGoodReversedScore_ShouldNotAddAdvice()
    {
        var childBook = BuildChildBook();
        var (answer, _, advice) = BuildAnswerWithAdvice(5, QuestionScoringType.Reversed);
        childBook.Child.Answers.Add(answer);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.DoesNotContain(advice, childBook.Advices);
    }

    [Fact]
    public async Task Run_ShouldCallUpdateOncePerChildBook()
    {
        var childBook = BuildChildBook();
        var (answer1, _, _) = BuildAnswerWithAdvice(5, QuestionScoringType.Default);
        var (answer2, _, _) = BuildAnswerWithAdvice(5, QuestionScoringType.Default);
        childBook.Child.Answers.Add(answer1);
        childBook.Child.Answers.Add(answer2);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        _repositoryMock.Verify(r => r.Update(childBook), Times.Once);
    }

    [Fact]
    public async Task Run_SameAdviceFromMultipleAnswers_ShouldNotDuplicate()
    {
        var childBook = BuildChildBook();
        var advice = new Advice("Do this.", AdminId, Guid.NewGuid());

        var question = new Question("Q?", AdminId, QuestionType.Agreement, QuestionScoringType.Default);
        typeof(Question).GetProperty(nameof(Question.Advices))!
            .SetValue(question, new List<Advice> { advice });

        for (var i = 0; i < 3; i++)
        {
            var answer = new Answer(5, question.Id, Guid.NewGuid(), Guid.NewGuid());
            typeof(Answer).GetProperty(nameof(Answer.Question))!.SetValue(answer, question);
            childBook.Child.Answers.Add(answer);
        }

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.Single(childBook.Advices);
    }
}

