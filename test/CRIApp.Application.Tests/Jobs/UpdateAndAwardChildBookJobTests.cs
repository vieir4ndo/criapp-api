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

public class UpdateAndAwardChildBookJobTests
{
    private readonly Mock<IChildBookRepository> _childBookRepositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly UpdateAndAwardChildBookJob _sut;

    private static IList<Award> CreateAllAwards() =>
    [
        new Award(AwardType.HabitBuilder, "Habit Builder"),
        new Award(AwardType.ConsistentParent, "Consistent Parent"),
        new Award(AwardType.SilverCognitiveDevelopment, "Silver Cognitive Development"),
        new Award(AwardType.SilverEmotionalBonding, "Silver Emotional Bonding"),
        new Award(AwardType.SilverDailyRoutines, "Silver Daily Routines"),
        new Award(AwardType.SilverSocialInteraction, "Silver Social Interaction"),
        new Award(AwardType.GoldParent, "Gold Parent"),
    ];

    public UpdateAndAwardChildBookJobTests()
    {
        _childBookRepositoryMock = new Mock<IChildBookRepository>();
        var checkpointRepositoryMock = new Mock<IRecommendationCheckpointRepository>();
        checkpointRepositoryMock
            .Setup(r => r.GetByChildBookIdAndDate(It.IsAny<Guid>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<RecommendationCheckpoint>());
        var awardRepositoryMock = new Mock<IAwardRepository>();
        awardRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(CreateAllAwards());
        _notificationServiceMock = new Mock<INotificationService>();
        _sut = new UpdateAndAwardChildBookJob(
            NullLogger<UpdateAndAwardChildBookJob>.Instance,
            _childBookRepositoryMock.Object,
            checkpointRepositoryMock.Object,
            awardRepositoryMock.Object,
            _notificationServiceMock.Object);
    }

    private static ChildBook BuildChildBook()
    {
        var prcId = Guid.NewGuid();
        var child = new Child("Child", new DateTime(2020, 1, 1), Sex.Male, 6, prcId, false);
        typeof(Child).GetProperty(nameof(Child.Answers))!.SetValue(child, new List<Answer>());

        var childBook = new ChildBook(child.Id);
        typeof(ChildBook).GetProperty(nameof(ChildBook.Child))!.SetValue(childBook, child);
        typeof(ChildBook).GetProperty(nameof(ChildBook.RecommendationCheckpoints))!
            .SetValue(childBook, new List<RecommendationCheckpoint>());
        return childBook;
    }

    private static Answer BuildAnswer(Guid questionId, int value, QuestionScoringType scoringType, QuestionGroup group = QuestionGroup.CognitiveDevelopment)
    {
        var question = new Question("Q?", QuestionType.Agreement, scoringType, group: group);
        var answer = new Answer(value, questionId, Guid.NewGuid(), Guid.NewGuid());
        typeof(Answer).GetProperty(nameof(Answer.Question))!.SetValue(answer, question);
        return answer;
    }

    private static RecommendationCheckpoint BuildCheckpoint(bool completed, DateTime date)
    {
        var checkpoint = new RecommendationCheckpoint(1, Guid.NewGuid(), completed);
        typeof(RecommendationCheckpoint)
            .GetProperty(nameof(RecommendationCheckpoint.Date), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!
            .SetValue(checkpoint, date);
        return checkpoint;
    }

    // --- Score update tests ---

    [Fact]
    public async Task Run_NoChildBooks_ShouldNotCallUpdate()
    {
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook>());

        await _sut.Run();

        _childBookRepositoryMock.Verify(r => r.Update(It.IsAny<ChildBook>()), Times.Never);
    }

    [Fact]
    public async Task Run_WithChildBooks_ShouldCallUpdateForEach()
    {
        var childBooks = new List<ChildBook> { BuildChildBook(), BuildChildBook(), BuildChildBook() };
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(childBooks);

        await _sut.Run();

        _childBookRepositoryMock.Verify(r => r.Update(It.IsAny<ChildBook>()), Times.Exactly(3));
    }

    [Fact]
    public async Task Run_ShouldCallGetAllOnce()
    {
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { BuildChildBook() });

        await _sut.Run();

        _childBookRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public async Task Run_ShouldCallUpdateWithCorrectChildBook()
    {
        var childBook = BuildChildBook();
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        _childBookRepositoryMock.Verify(r => r.Update(childBook), Times.Once);
    }

    // --- Award tests ---

    [Fact]
    public async Task Run_FourDistinctCompletedDaysInSevenDayWindow_ShouldGrantHabitBuilder()
    {
        var childBook = BuildChildBook();
        var today = DateTime.UtcNow.Date;
        childBook.RecommendationCheckpoints.Add(BuildCheckpoint(true, today.AddDays(-6)));
        childBook.RecommendationCheckpoints.Add(BuildCheckpoint(true, today.AddDays(-5)));
        childBook.RecommendationCheckpoints.Add(BuildCheckpoint(true, today.AddDays(-4)));
        childBook.RecommendationCheckpoints.Add(BuildCheckpoint(true, today.AddDays(-3)));
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync([childBook]);

        await _sut.Run();

        Assert.Contains(childBook.Awards, a => a.Type == AwardType.HabitBuilder);
    }

    [Fact]
    public async Task Run_ThreeDistinctCompletedDaysInSevenDayWindow_ShouldNotGrantHabitBuilder()
    {
        var childBook = BuildChildBook();
        var today = DateTime.UtcNow.Date;
        childBook.RecommendationCheckpoints.Add(BuildCheckpoint(true, today.AddDays(-6)));
        childBook.RecommendationCheckpoints.Add(BuildCheckpoint(true, today.AddDays(-4)));
        childBook.RecommendationCheckpoints.Add(BuildCheckpoint(true, today.AddDays(-2)));
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync([childBook]);

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.HabitBuilder);
    }

    [Fact]
    public async Task Run_FourDaysSpreadOverMoreThanSevenDays_ShouldNotGrantHabitBuilder()
    {
        var childBook = BuildChildBook();
        var today = DateTime.UtcNow.Date;
        childBook.RecommendationCheckpoints.Add(BuildCheckpoint(true, today.AddDays(-20)));
        childBook.RecommendationCheckpoints.Add(BuildCheckpoint(true, today.AddDays(-15)));
        childBook.RecommendationCheckpoints.Add(BuildCheckpoint(true, today.AddDays(-10)));
        childBook.RecommendationCheckpoints.Add(BuildCheckpoint(true, today.AddDays(-5)));
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync([childBook]);

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.HabitBuilder);
    }

    [Fact]
    public async Task Run_FourDaysButNotCompleted_ShouldNotGrantHabitBuilder()
    {
        var childBook = BuildChildBook();
        var today = DateTime.UtcNow.Date;
        childBook.RecommendationCheckpoints.Add(BuildCheckpoint(false, today.AddDays(-6)));
        childBook.RecommendationCheckpoints.Add(BuildCheckpoint(false, today.AddDays(-5)));
        childBook.RecommendationCheckpoints.Add(BuildCheckpoint(false, today.AddDays(-4)));
        childBook.RecommendationCheckpoints.Add(BuildCheckpoint(false, today.AddDays(-3)));
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync([childBook]);

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.HabitBuilder);
    }

    [Fact]
    public async Task Run_SevenDistinctCompletedDays_ShouldGrantConsistentParent()
    {
        var childBook = BuildChildBook();
        var today = DateTime.UtcNow.Date;
        for (var i = 1; i <= 7; i++)
            childBook.RecommendationCheckpoints.Add(BuildCheckpoint(true, today.AddDays(-i)));
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync([childBook]);

        await _sut.Run();

        Assert.Contains(childBook.Awards, a => a.Type == AwardType.ConsistentParent);
    }

    [Fact]
    public async Task Run_SixDistinctCompletedDays_ShouldNotGrantConsistentParent()
    {
        var childBook = BuildChildBook();
        var today = DateTime.UtcNow.Date;
        for (var i = 1; i <= 6; i++)
            childBook.RecommendationCheckpoints.Add(BuildCheckpoint(true, today.AddDays(-i)));
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync([childBook]);

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.ConsistentParent);
    }

    [Fact]
    public async Task Run_AllGroupAnswersPositive_ShouldGrantSilverCognitiveDevelopment()
    {
        var childBook = BuildChildBook();
        for (var i = 0; i < 3; i++)
            childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 5, QuestionScoringType.Default));
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync([childBook]);

        await _sut.Run();

        Assert.Contains(childBook.Awards, a => a.Type == AwardType.SilverCognitiveDevelopment);
    }

    [Fact]
    public async Task Run_NotAllGroupAnswersPositive_ShouldNotGrantSilverCognitiveDevelopment()
    {
        var childBook = BuildChildBook();
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 5, QuestionScoringType.Default));
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default));
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync([childBook]);

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.SilverCognitiveDevelopment);
    }

    [Fact]
    public async Task Run_NoAnswersInGroup_ShouldNotGrantSilverMedal()
    {
        var childBook = BuildChildBook();
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync([childBook]);

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.SilverCognitiveDevelopment);
    }

    [Fact]
    public async Task Run_AllFourSilverMedalsPresent_ShouldGrantGoldParent()
    {
        var childBook = BuildChildBook();
        var allAwards = CreateAllAwards();
        foreach (var silver in allAwards.Where(a => a.Type is
            AwardType.SilverCognitiveDevelopment or
            AwardType.SilverEmotionalBonding or
            AwardType.SilverDailyRoutines or
            AwardType.SilverSocialInteraction))
        {
            childBook.AddAwardIfNeeded(silver);
        }
        foreach (var group in Enum.GetValues<QuestionGroup>())
            childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 5, QuestionScoringType.Default, group));
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync([childBook]);

        await _sut.Run();

        Assert.Contains(childBook.Awards, a => a.Type == AwardType.GoldParent);
    }

    [Fact]
    public async Task Run_ThreeSilverMedals_ShouldNotGrantGoldParent()
    {
        var childBook = BuildChildBook();
        var allAwards = CreateAllAwards();
        foreach (var silver in allAwards.Where(a => a.Type is
            AwardType.SilverCognitiveDevelopment or
            AwardType.SilverEmotionalBonding or
            AwardType.SilverDailyRoutines))
        {
            childBook.AddAwardIfNeeded(silver);
        }
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync([childBook]);

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.GoldParent);
    }

    [Fact]
    public async Task Run_AwardAlreadyGranted_ShouldNotDuplicate()
    {
        var childBook = BuildChildBook();
        var today = DateTime.UtcNow.Date;
        for (var i = 1; i <= 7; i++)
            childBook.RecommendationCheckpoints.Add(BuildCheckpoint(true, today.AddDays(-i)));
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync([childBook]);

        await _sut.Run();
        await _sut.Run();

        Assert.Single(childBook.Awards.Where(a => a.Type == AwardType.ConsistentParent));
    }

    [Fact]
    public async Task Run_NewAwardGranted_ShouldCreateNewAwardNotification()
    {
        var childBook = BuildChildBook();
        var today = DateTime.UtcNow.Date;
        for (var i = 1; i <= 7; i++)
            childBook.RecommendationCheckpoints.Add(BuildCheckpoint(true, today.AddDays(-i)));
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync([childBook]);

        await _sut.Run();

        _notificationServiceMock.Verify(
            r => r.CreateIfNotExists(It.IsAny<Guid>(), NotificationType.NewAward),
            Times.Once);
    }

    [Fact]
    public async Task Run_NoNewAwards_ShouldNotCreateNewAwardNotification()
    {
        var childBook = BuildChildBook();
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync([childBook]);

        await _sut.Run();

        _notificationServiceMock.Verify(
            r => r.CreateIfNotExists(It.IsAny<Guid>(), NotificationType.NewAward),
            Times.Never);
    }
}
