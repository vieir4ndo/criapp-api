using CRIApp.Application.Abstractions;
using CRIApp.Application.Jobs;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Reflection;
using Xunit;

namespace CRIApp.Application.Tests.Jobs;

public class SaveUsersRecommendationsTests
{
    private readonly Mock<IChildBookRepository> _repositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly SaveUsersRecommendations _sut;

    private static readonly Guid AdminId = Guid.NewGuid();

    public SaveUsersRecommendationsTests()
    {
        _repositoryMock = new Mock<IChildBookRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _sut = new SaveUsersRecommendations(
            NullLogger<SaveUsersRecommendations>.Instance,
            _repositoryMock.Object,
            _notificationServiceMock.Object);
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

    private static (Answer answer, Question question, Recommendation recommendation) BuildAnswerWithRecommendation(
        int value, QuestionScoringType scoringType)
    {
        var recommendation = new Recommendation("Do this.", AdminId, Guid.NewGuid());
        var question = new Question("Q?", AdminId, QuestionType.Agreement, scoringType);
        typeof(Question).GetProperty(nameof(Question.Recommendations))!
            .SetValue(question, new List<Recommendation> { recommendation });

        var answer = new Answer(value, question.Id, Guid.NewGuid(), Guid.NewGuid());
        typeof(Answer).GetProperty(nameof(Answer.Question))!.SetValue(answer, question);

        return (answer, question, recommendation);
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
        var (answer, _, recommendation) = BuildAnswerWithRecommendation(1, QuestionScoringType.Default);
        childBook.Child.Answers.Add(answer);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.Contains(recommendation, childBook.Recommendations);
    }

    [Fact]
    public async Task Run_AnswerWithGoodDefaultScore_ShouldNotAddAdvice()
    {
        var childBook = BuildChildBook();
        var (answer, _, recommendation) = BuildAnswerWithRecommendation(5, QuestionScoringType.Default);
        childBook.Child.Answers.Add(answer);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.DoesNotContain(recommendation, childBook.Recommendations);
    }

    [Fact]
    public async Task Run_AnswerWithBadReversedScore_ShouldAddAdvice()
    {
        var childBook = BuildChildBook();
        var (answer, _, recommendation) = BuildAnswerWithRecommendation(5, QuestionScoringType.Reversed);
        childBook.Child.Answers.Add(answer);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.Contains(recommendation, childBook.Recommendations);
    }

    [Fact]
    public async Task Run_AnswerWithGoodDefaultScore_ShouldNotCallUpdate()
    {
        var childBook = BuildChildBook();
        var (answer, _, _) = BuildAnswerWithRecommendation(5, QuestionScoringType.Default);
        childBook.Child.Answers.Add(answer);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        _repositoryMock.Verify(r => r.Update(It.IsAny<ChildBook>()), Times.Never);
    }

    [Fact]
    public async Task Run_AnswerWithGoodReversedScore_ShouldNotAddAdvice()
    {
        var childBook = BuildChildBook();
        var (answer, _, recommendation) = BuildAnswerWithRecommendation(1, QuestionScoringType.Reversed);
        childBook.Child.Answers.Add(answer);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.DoesNotContain(recommendation, childBook.Recommendations);
    }

    [Fact]
    public async Task Run_ShouldCallUpdateOncePerChildBook()
    {
        var childBook = BuildChildBook();
        var (answer1, _, _) = BuildAnswerWithRecommendation(1, QuestionScoringType.Default);
        var (answer2, _, _) = BuildAnswerWithRecommendation(1, QuestionScoringType.Default);
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
        var recommendation = new Recommendation("Do this.", AdminId, Guid.NewGuid());

        var question = new Question("Q?", AdminId, QuestionType.Agreement, QuestionScoringType.Default);
        typeof(Question).GetProperty(nameof(Question.Recommendations))!
            .SetValue(question, new List<Recommendation> { recommendation });

        for (var i = 0; i < 3; i++)
        {
            var answer = new Answer(1, question.Id, Guid.NewGuid(), Guid.NewGuid());
            typeof(Answer).GetProperty(nameof(Answer.Question))!.SetValue(answer, question);
            childBook.Child.Answers.Add(answer);
        }

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.Single(childBook.Recommendations);
    }

    [Fact]
    public async Task Run_NewRecommendationsAdded_ShouldCreateNewRecommendationsNotification()
    {
        var childBook = BuildChildBook();
        var (answer, _, _) = BuildAnswerWithRecommendation(1, QuestionScoringType.Default);
        childBook.Child.Answers.Add(answer);
        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        _notificationServiceMock.Verify(
            r => r.CreateIfNotExists(It.IsAny<Guid>(), NotificationType.NewRecommendations),
            Times.Once);
    }

    [Fact]
    public async Task Run_NoNewRecommendations_ShouldNotCreateNotification()
    {
        var childBook = BuildChildBook();
        var (answer, _, _) = BuildAnswerWithRecommendation(5, QuestionScoringType.Default);
        childBook.Child.Answers.Add(answer);
        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        _notificationServiceMock.Verify(
            r => r.CreateIfNotExists(It.IsAny<Guid>(), It.IsAny<NotificationType>()),
            Times.Never);
    }
}

