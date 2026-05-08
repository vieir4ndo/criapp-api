using CRIApp.Application.Jobs;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Jobs;

public class SaveUsersRecommendationsTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly SaveUsersRecommendations _sut;

    private static readonly Guid AdminId = Guid.NewGuid();

    public SaveUsersRecommendationsTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _sut = new SaveUsersRecommendations(NullLogger<SaveUsersRecommendations>.Instance, _repositoryMock.Object);
    }

    private static User BuildUser()
        => new User("Parent", new DateTime(1985, 1, 1), Sex.Male, "parent@domain.com", "pass123", FamilyType.HeterosexualCouple);

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
    public async Task Run_NoUsers_ShouldNotCallUpdate()
    {
        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User>());

        await _sut.Run();

        _repositoryMock.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Run_UserWithNoAnswers_ShouldNotCallUpdate()
    {
        var user = BuildUser();
        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        _repositoryMock.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Run_AnswerWithBadDefaultScore_ShouldAddRecommendation()
    {
        var user = BuildUser();
        var (answer, _, recommendation) = BuildAnswerWithRecommendation(5, QuestionScoringType.Default);
        user.Answers.Add(answer);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.Contains(recommendation, user.Recommendations);
    }

    [Fact]
    public async Task Run_AnswerWithGoodDefaultScore_ShouldNotAddRecommendation()
    {
        var user = BuildUser();
        var (answer, _, recommendation) = BuildAnswerWithRecommendation(1, QuestionScoringType.Default);
        user.Answers.Add(answer);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.DoesNotContain(recommendation, user.Recommendations);
    }

    [Fact]
    public async Task Run_AnswerWithBadReversedScore_ShouldAddRecommendation()
    {
        var user = BuildUser();
        var (answer, _, recommendation) = BuildAnswerWithRecommendation(1, QuestionScoringType.Reversed);
        user.Answers.Add(answer);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.Contains(recommendation, user.Recommendations);
    }

    [Fact]
    public async Task Run_AnswerWithGoodDefaultScore_ShouldNotCallUpdate()
    {
        var user = BuildUser();
        var (answer, _, _) = BuildAnswerWithRecommendation(1, QuestionScoringType.Default);
        user.Answers.Add(answer);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        _repositoryMock.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Run_AnswerWithGoodReversedScore_ShouldNotAddRecommendation()
    {
        var user = BuildUser();
        var (answer, _, recommendation) = BuildAnswerWithRecommendation(5, QuestionScoringType.Reversed);
        user.Answers.Add(answer);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.DoesNotContain(recommendation, user.Recommendations);
    }

    [Fact]
    public async Task Run_ShouldCallUpdateOncePerUser()
    {
        var user = BuildUser();
        var (answer1, _, _) = BuildAnswerWithRecommendation(5, QuestionScoringType.Default);
        var (answer2, _, _) = BuildAnswerWithRecommendation(5, QuestionScoringType.Default);
        user.Answers.Add(answer1);
        user.Answers.Add(answer2);

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        _repositoryMock.Verify(r => r.Update(user), Times.Once);
    }

    [Fact]
    public async Task Run_SameRecommendationFromMultipleAnswers_ShouldNotDuplicate()
    {
        var user = BuildUser();
        var recommendation = new Recommendation("Do this.", AdminId, Guid.NewGuid());

        var question = new Question("Q?", AdminId, QuestionType.Agreement, QuestionScoringType.Default);
        typeof(Question).GetProperty(nameof(Question.Recommendations))!
            .SetValue(question, new List<Recommendation> { recommendation });

        for (var i = 0; i < 3; i++)
        {
            var answer = new Answer(5, question.Id, Guid.NewGuid(), Guid.NewGuid());
            typeof(Answer).GetProperty(nameof(Answer.Question))!.SetValue(answer, question);
            user.Answers.Add(answer);
        }

        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.Single(user.Recommendations);
    }
}
