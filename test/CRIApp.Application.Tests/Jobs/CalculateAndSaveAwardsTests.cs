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
    private readonly Mock<IChildBookRepository> _childBookRepositoryMock;
    private readonly Mock<IAwardRepository> _awardRepositoryMock;
    private readonly CalculateAndSaveAwards _sut;

    private static readonly Guid AdminId = Guid.NewGuid();

    public CalculateAndSaveAwardsTests()
    {
        _childBookRepositoryMock = new Mock<IChildBookRepository>();
        _awardRepositoryMock = new Mock<IAwardRepository>();
        _sut = new CalculateAndSaveAwards(
            NullLogger<CalculateAndSaveAwards>.Instance,
            _childBookRepositoryMock.Object,
            _awardRepositoryMock.Object);
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

    private static Answer BuildAnswer(Guid questionId, int value, QuestionScoringType scoringType, DateTime? createdAt = null)
    {
        var question = new Question("Q?", AdminId, QuestionType.Agreement, scoringType);

        var answer = new Answer(value, questionId, Guid.NewGuid(), Guid.NewGuid());
        typeof(Answer).GetProperty(nameof(Answer.Question))!.SetValue(answer, question);

        if (createdAt.HasValue)
            typeof(Answer).GetProperty(nameof(Answer.CreatedAt))!.SetValue(answer, createdAt.Value);

        return answer;
    }

    private static void SetScore(ChildBook childBook, int score)
        => typeof(ChildBook).GetProperty(nameof(ChildBook.Score))!.SetValue(childBook, score);


    [Fact]
    public async Task Run_ChildBookWithFiveDistinctQuestions_ShouldGrantFirstStepsAward()
    {
        var childBook = BuildChildBook();
        for (var i = 0; i < 5; i++)
            childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default));

        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.Contains(childBook.Awards, a => a.Type == AwardType.FirstSteps);
    }

    [Fact]
    public async Task Run_ChildBookWithFourDistinctQuestions_ShouldNotGrantFirstStepsAward()
    {
        var childBook = BuildChildBook();
        for (var i = 0; i < 4; i++)
            childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default));

        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.FirstSteps);
    }

    [Fact]
    public async Task Run_SameQuestionAnsweredMultipleTimes_ShouldNotGrantFirstStepsAward()
    {
        var childBook = BuildChildBook();
        var sameQuestionId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
            childBook.Child.Answers.Add(BuildAnswer(sameQuestionId, 1, QuestionScoringType.Default));

        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.FirstSteps);
    }

    [Fact]
    public async Task Run_ChildBookWithThreeConsecutiveDays_ShouldGrantEngagedFamilyAward()
    {
        var childBook = BuildChildBook();
        var today = DateTime.UtcNow.Date;
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-4)));
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-3)));
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-2)));

        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.Contains(childBook.Awards, a => a.Type == AwardType.EngagedFamily);
    }

    [Fact]
    public async Task Run_ChildBookWithNonConsecutiveDays_ShouldNotGrantEngagedFamilyAward()
    {
        var childBook = BuildChildBook();
        var today = DateTime.UtcNow.Date;
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-5)));
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-3)));
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-1)));

        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.EngagedFamily);
    }

    [Fact]
    public async Task Run_ChildBookWithTwoDays_ShouldNotGrantEngagedFamilyAward()
    {
        var childBook = BuildChildBook();
        var today = DateTime.UtcNow.Date;
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-2)));
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, today.AddDays(-1)));

        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.EngagedFamily);
    }

    [Fact]
    public async Task Run_ScoreIncreasedBy20Percent_ShouldGrantOvercomingAward()
    {
        var childBook = BuildChildBook();
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        var dayBefore = today.AddDays(-2);

        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, dayBefore));
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 5, QuestionScoringType.Default, dayBefore));
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, yesterday));

        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.Contains(childBook.Awards, a => a.Type == AwardType.Overcoming);
    }

    [Fact]
    public async Task Run_ScoreIncreasedByLessThan20Percent_ShouldNotGrantOvercomingAward()
    {
        var childBook = BuildChildBook();
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        var dayBefore = today.AddDays(-2);

        for (var i = 0; i < 9; i++)
            childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, dayBefore));
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 5, QuestionScoringType.Default, dayBefore));
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, yesterday));

        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.Overcoming);
    }

    [Fact]
    public async Task Run_NoPreviousDayScore_ShouldNotGrantOvercomingAward()
    {
        var childBook = BuildChildBook();
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, yesterday));

        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.Overcoming);
    }

    [Fact]
    public async Task Run_ScoreAbove80_ShouldGrantPositiveEnvironmentAward()
    {
        var childBook = BuildChildBook();
        SetScore(childBook, 85);
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.Contains(childBook.Awards, a => a.Type == AwardType.PositiveEnvironment);
    }

    [Fact]
    public async Task Run_ScoreExactly80_ShouldNotGrantPositiveEnvironmentAward()
    {
        var childBook = BuildChildBook();
        SetScore(childBook, 80);
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.PositiveEnvironment);
    }

    [Fact]
    public async Task Run_ScoreBelow80_ShouldNotGrantPositiveEnvironmentAward()
    {
        var childBook = BuildChildBook();
        SetScore(childBook, 50);
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.PositiveEnvironment);
    }

    [Fact]
    public async Task Run_ScoreIncreasedAtAll_ShouldGrantSmallProgressAward()
    {
        var childBook = BuildChildBook();
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        var dayBefore = today.AddDays(-2);

        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, dayBefore));
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 5, QuestionScoringType.Default, dayBefore));
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, yesterday));

        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.Contains(childBook.Awards, a => a.Type == AwardType.SmallProgress);
    }

    [Fact]
    public async Task Run_ScoreNotIncreased_ShouldNotGrantSmallProgressAward()
    {
        var childBook = BuildChildBook();
        var dayBefore = DateTime.UtcNow.Date.AddDays(-2);
        childBook.Child.Answers.Add(BuildAnswer(Guid.NewGuid(), 1, QuestionScoringType.Default, dayBefore));

        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        Assert.DoesNotContain(childBook.Awards, a => a.Type == AwardType.SmallProgress);
    }

    [Fact]
    public async Task Run_AwardAlreadyGranted_ShouldNotDuplicate()
    {
        var childBook = BuildChildBook();
        SetScore(childBook, 90);
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();
        await _sut.Run();

        Assert.Single(childBook.Awards.Where(a => a.Type == AwardType.PositiveEnvironment));
    }

    [Fact]
    public async Task Run_TwoChildBooksWithNewAwards_ShouldCallCreateRangeForEach()
    {
        var childBook1 = BuildChildBook();
        SetScore(childBook1, 85);
        var childBook2 = BuildChildBook();
        SetScore(childBook2, 85);
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook1, childBook2 });

        await _sut.Run();

        _awardRepositoryMock.Verify(r => r.CreateRange(It.IsAny<IEnumerable<Award>>()), Times.Exactly(2));
    }
}

