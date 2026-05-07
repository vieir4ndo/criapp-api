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

public class CalculateAndSaveAwardsTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly CalculateAndSaveAwards _sut;

    private static readonly Guid AdminId = Guid.NewGuid();

    public CalculateAndSaveAwardsTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userServiceMock = new Mock<IUserService>();
        _sut = new CalculateAndSaveAwards(
            NullLogger<CalculateAndSaveAwards>.Instance,
            _userRepositoryMock.Object,
            _userServiceMock.Object);
    }

    private static User BuildUser(bool isAdmin = false)
    {
        var user = new User("Parent", new DateTime(1985, 1, 1), Sex.Male, "parent@domain.com", "pass123", FamilyType.HeterosexualCouple);
        if (isAdmin)
            typeof(User).GetProperty(nameof(User.IsAdmin))!.SetValue(user, true);
        return user;
    }

    private static Answer BuildAnswer(Guid questionId, int value, QuestionScoringType scoringType, DateTime? createdAt = null)
    {
        var question = new Question("Q?", AdminId, QuestionType.Agreement, scoringType);

        var answer = new Answer(value, questionId, Guid.NewGuid(), Guid.NewGuid());
        typeof(Answer).GetProperty(nameof(Answer.Question))!.SetValue(answer, question);

        if (createdAt.HasValue)
            typeof(Answer).GetProperty(nameof(Answer.CreatedAt))!.SetValue(answer, createdAt.Value);

        return answer;
    }

    private static void SetScore(User user, int score)
        => typeof(User).GetProperty(nameof(User.Score))!.SetValue(user, score);


    [Fact]
    public async Task Run_AdminUser_ShouldNotCallUpdate()
    {
        var admin = BuildUser(isAdmin: true);
        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { admin });

        await _sut.Run();

        _userRepositoryMock.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
    }


    [Fact]
    public async Task Run_UserWithFiveDistinctQuestions_ShouldGrantFirstStepsAward()
    {
        var user = BuildUser();
        for (var i = 0; i < 5; i++)
            user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default));

        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.Contains(user.Awards, a => a.Type == AwardType.FirstSteps);
    }

    [Fact]
    public async Task Run_UserWithFourDistinctQuestions_ShouldNotGrantFirstStepsAward()
    {
        var user = BuildUser();
        for (var i = 0; i < 4; i++)
            user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default));

        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.DoesNotContain(user.Awards, a => a.Type == AwardType.FirstSteps);
    }

    [Fact]
    public async Task Run_UserWithSameQuestionAnsweredMultipleTimes_ShouldNotGrantFirstStepsAward()
    {
        var user = BuildUser();
        var sameQuestionId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
            user.Answers.Add(BuildAnswer(sameQuestionId, 1, QuestionScoringType.Default));

        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.DoesNotContain(user.Awards, a => a.Type == AwardType.FirstSteps);
    }


    [Fact]
    public async Task Run_UserWithThreeConsecutiveDays_ShouldGrantEngagedFamilyAward()
    {
        var user = BuildUser();
        var today = DateTime.UtcNow.Date;
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-4)));
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-3)));
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-2)));

        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.Contains(user.Awards, a => a.Type == AwardType.EngagedFamily);
    }

    [Fact]
    public async Task Run_UserWithNonConsecutiveDays_ShouldNotGrantEngagedFamilyAward()
    {
        var user = BuildUser();
        var today = DateTime.UtcNow.Date;
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-5)));
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-3)));
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-1)));

        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.DoesNotContain(user.Awards, a => a.Type == AwardType.EngagedFamily);
    }

    [Fact]
    public async Task Run_UserWithTwoDays_ShouldNotGrantEngagedFamilyAward()
    {
        var user = BuildUser();
        var today = DateTime.UtcNow.Date;
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-2)));
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-1)));

        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.DoesNotContain(user.Awards, a => a.Type == AwardType.EngagedFamily);
    }


    [Fact]
    public async Task Run_ScoreIncreasedBy20Percent_ShouldGrantOvercomingAward()
    {
        var user = BuildUser();
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        var dayBefore = today.AddDays(-2);

        // dayBefore: 1/1 = 100% → score = 100
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, dayBefore));
        // yesterday adds 4 more correct → 5/5 = 100, but we need to simulate an increase
        // Use dayBefore: 1 answer correct = score 100
        // yesterday: only an extra answer to keep score same... 
        // Better: dayBefore 1 correct out of 2 = 50, yesterday 1 correct out of 1 = 100
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 5, QuestionScoringType.Default, dayBefore)); // wrong
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, yesterday)); // correct

        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.Contains(user.Awards, a => a.Type == AwardType.Overcoming);
    }

    [Fact]
    public async Task Run_ScoreIncreasedByLessThan20Percent_ShouldNotGrantOvercomingAward()
    {
        var user = BuildUser();
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        var dayBefore = today.AddDays(-2);

        // dayBefore: 9/10 = 90, yesterday adds 1 wrong → 9/11 ≈ 81 — score decreased, skip
        // dayBefore: 9/10 = 90, yesterday same + 1 correct → 10/11 ≈ 90 — no 20% increase
        for (var i = 0; i < 9; i++)
            user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, dayBefore));
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 5, QuestionScoringType.Default, dayBefore)); // wrong → 9/10 = 90
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, yesterday)); // 10/11 ≈ 90

        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.DoesNotContain(user.Awards, a => a.Type == AwardType.Overcoming);
    }

    [Fact]
    public async Task Run_NoPreviousDayScore_ShouldNotGrantOvercomingAward()
    {
        var user = BuildUser();
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, yesterday));

        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.DoesNotContain(user.Awards, a => a.Type == AwardType.Overcoming);
    }


    [Fact]
    public async Task Run_ScoreAbove80_ShouldGrantPositiveEnvironmentAward()
    {
        var user = BuildUser();
        SetScore(user, 85);
        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.Contains(user.Awards, a => a.Type == AwardType.PositiveEnvironment);
    }

    [Fact]
    public async Task Run_ScoreExactly80_ShouldNotGrantPositiveEnvironmentAward()
    {
        var user = BuildUser();
        SetScore(user, 80);
        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.DoesNotContain(user.Awards, a => a.Type == AwardType.PositiveEnvironment);
    }

    [Fact]
    public async Task Run_ScoreBelow80_ShouldNotGrantPositiveEnvironmentAward()
    {
        var user = BuildUser();
        SetScore(user, 50);
        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.DoesNotContain(user.Awards, a => a.Type == AwardType.PositiveEnvironment);
    }


    [Fact]
    public async Task Run_ScoreIncreasedAtAll_ShouldGrantSmallProgressAward()
    {
        var user = BuildUser();
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        var dayBefore = today.AddDays(-2);

        // dayBefore: 1 wrong = score 0 → won't trigger (scoreDayBefore must be > 0 for Overcoming, but SmallProgress just needs increase)
        // dayBefore: 1/2 = 50, yesterday: 1/1 = 100
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, dayBefore));  // correct
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 5, QuestionScoringType.Default, dayBefore));  // wrong → 1/2 = 50
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, yesterday));  // correct → cumulative 2/3 ≈ 66

        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.Contains(user.Awards, a => a.Type == AwardType.SmallProgress);
    }

    [Fact]
    public async Task Run_ScoreNotIncreased_ShouldNotGrantSmallProgressAward()
    {
        var user = BuildUser();
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        var dayBefore = today.AddDays(-2);

        // dayBefore: 1/1 = 100, yesterday: no new answers → still 100, no increase
        user.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, dayBefore));

        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();

        Assert.DoesNotContain(user.Awards, a => a.Type == AwardType.SmallProgress);
    }


    [Fact]
    public async Task Run_AwardAlreadyGranted_ShouldNotDuplicate()
    {
        var user = BuildUser();
        SetScore(user, 90);
        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<User> { user });

        await _sut.Run();
        await _sut.Run();

        Assert.Single(user.Awards.Where(a => a.Type == AwardType.PositiveEnvironment));
    }


    [Fact]
    public async Task Run_WithNonAdminUsers_ShouldCallUpdateForEachUser()
    {
        var users = new List<User> { BuildUser(), BuildUser() };
        _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(users);

        await _sut.Run();

        _userRepositoryMock.Verify(r => r.Update(It.IsAny<User>()), Times.Exactly(2));
    }
}
