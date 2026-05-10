using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using System.Reflection;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class ChildBookTests
{
    private static readonly Guid AdminId = Guid.NewGuid();

    private static ChildBook CreateChildBook()
    {
        var prcId = Guid.NewGuid();
        var child = new Child("Child", new DateTime(2020, 1, 1), Sex.Male, 6, prcId, false);
        typeof(Child).GetProperty(nameof(Child.Answers))!.SetValue(child, new List<Answer>());

        var childBook = new ChildBook(child.Id);
        typeof(ChildBook).GetProperty(nameof(ChildBook.Child))!.SetValue(childBook, child);
        return childBook;
    }

    private static Answer BuildAnswerWithQuestion(int value, QuestionScoringType scoringType)
    {
        var question = new Question("Sample question?", AdminId, QuestionType.Agreement, scoringType);
        var answer = new Answer(value, question.Id, Guid.NewGuid(), Guid.NewGuid());
        typeof(Answer).GetProperty(nameof(Answer.Question))!.SetValue(answer, question);
        return answer;
    }

    [Fact]
    public void Create_ShouldInitializeScoreToZero()
    {
        var childBook = CreateChildBook();

        Assert.Equal(0, childBook.Score);
    }

    [Fact]
    public void UpdateScore_NoAnswers_ShouldSetScoreToZero()
    {
        var childBook = CreateChildBook();

        childBook.UpdateScore(childBook.Child.Answers, []);

        Assert.Equal(0, childBook.Score);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void UpdateScore_DefaultScoringWithCountableValue_ShouldScore100(int value)
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(value, QuestionScoringType.Default));

        childBook.UpdateScore(childBook.Child.Answers, []);

        Assert.Equal(100, childBook.Score);
    }

    [Fact]
    public void UpdateScore_DefaultScoringWithValue5_ShouldNotCount()
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(5, QuestionScoringType.Default));

        childBook.UpdateScore(childBook.Child.Answers, []);

        Assert.Equal(0, childBook.Score);
    }

    [Fact]
    public void UpdateScore_ReversedScoringWithValue5_ShouldScore100()
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(5, QuestionScoringType.Reversed));

        childBook.UpdateScore(childBook.Child.Answers, []);

        Assert.Equal(100, childBook.Score);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void UpdateScore_ReversedScoringWithLowValue_ShouldNotCount(int value)
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(value, QuestionScoringType.Reversed));

        childBook.UpdateScore(childBook.Child.Answers, []);

        Assert.Equal(0, childBook.Score);
    }

    [Fact]
    public void UpdateScore_MixedScoringTypes_ShouldCountCorrectly()
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(3, QuestionScoringType.Default));
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(5, QuestionScoringType.Default));
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(5, QuestionScoringType.Reversed));
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(2, QuestionScoringType.Reversed));

        childBook.UpdateScore(childBook.Child.Answers, []);

        Assert.Equal(50, childBook.Score);
    }

    [Fact]
    public void UpdateScore_ShouldUpdateScoreLastUpdatedAt()
    {
        var childBook = CreateChildBook();
        var before = DateTime.UtcNow;

        childBook.UpdateScore(childBook.Child.Answers, []);

        Assert.True(childBook.ScoreLastUpdatedAt >= before);
    }

    private static Recommendation BuildRecommendation()
    {
        var rec = new Recommendation("Recommendation", AdminId, Guid.NewGuid());
        typeof(Recommendation).GetProperty(nameof(Recommendation.ChildBooks))!.SetValue(rec, new List<ChildBook>());
        return rec;
    }

    private static RecommendationCheckpoint BuildCheckpoint(int recommendationId, Guid childBookId, bool completed)
        => new(recommendationId, childBookId, completed);

    [Fact]
    public void UpdateScore_NoRecommendations_ShouldApplyFullAnswersWeight()
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(3, QuestionScoringType.Default));

        childBook.UpdateScore(childBook.Child.Answers, []);

        Assert.Equal(100, childBook.Score);
    }

    [Fact]
    public void UpdateScore_WithRecommendationsAndNoCheckpoints_ShouldApply70AnswersWeight()
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(3, QuestionScoringType.Default));
        childBook.AddRecommendationsIfNeeded([BuildRecommendation()]);

        childBook.UpdateScore(childBook.Child.Answers, []);

        Assert.Equal(70, childBook.Score);
    }

    [Fact]
    public void UpdateScore_WithAllCheckpointsCompleted_ShouldScore100()
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(3, QuestionScoringType.Default));
        childBook.AddRecommendationsIfNeeded([BuildRecommendation()]);
        var checkpoint = BuildCheckpoint(1, childBook.Id, completed: true);

        childBook.UpdateScore(childBook.Child.Answers, [checkpoint]);

        Assert.Equal(100, childBook.Score);
    }

    [Fact]
    public void UpdateScore_WithPartialCheckpoints_ShouldApplyPartialCheckpointWeight()
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(3, QuestionScoringType.Default));
        childBook.AddRecommendationsIfNeeded([BuildRecommendation(), BuildRecommendation()]);
        var completedCheckpoint = BuildCheckpoint(1, childBook.Id, completed: true);
        var incompleteCheckpoint = BuildCheckpoint(2, childBook.Id, completed: false);

        childBook.UpdateScore(childBook.Child.Answers, [completedCheckpoint, incompleteCheckpoint]);

        Assert.Equal(85, childBook.Score);
    }

    [Fact]
    public void AddAwardIfNeeded_NewAwardType_ShouldAddAward()
    {
        var childBook = CreateChildBook();
        var award = new Award(AwardType.FirstSteps, "First Steps");

        childBook.AddAwardIfNeeded(award);

        Assert.Single(childBook.Awards);
        Assert.Equal(AwardType.FirstSteps, childBook.Awards[0].Type);
    }

    [Fact]
    public void AddAwardIfNeeded_SameAwardTypeTwice_ShouldNotDuplicate()
    {
        var childBook = CreateChildBook();
        var award = new Award(AwardType.FirstSteps, "First Steps");

        childBook.AddAwardIfNeeded(award);
        childBook.AddAwardIfNeeded(award);

        Assert.Single(childBook.Awards);
    }

    [Fact]
    public void AddAwardIfNeeded_DifferentAwardTypes_ShouldAddBoth()
    {
        var childBook = CreateChildBook();

        childBook.AddAwardIfNeeded(new Award(AwardType.FirstSteps, "First Steps"));
        childBook.AddAwardIfNeeded(new Award(AwardType.EngagedFamily, "Engaged Family"));

        Assert.Equal(2, childBook.Awards.Count);
    }

    [Fact]
    public void AddAwardIfNeeded_AllFiveTypes_ShouldAddAllFive()
    {
        var childBook = CreateChildBook();

        childBook.AddAwardIfNeeded(new Award(AwardType.FirstSteps, "First Steps"));
        childBook.AddAwardIfNeeded(new Award(AwardType.EngagedFamily, "Engaged Family"));
        childBook.AddAwardIfNeeded(new Award(AwardType.Overcoming, "Overcoming"));
        childBook.AddAwardIfNeeded(new Award(AwardType.PositiveEnvironment, "Positive Environment"));
        childBook.AddAwardIfNeeded(new Award(AwardType.SmallProgress, "Small Progress"));

        Assert.Equal(5, childBook.Awards.Count);
    }

    [Fact]
    public void AddRecommendationsIfNeeded_NewRecommendation_ShouldAddIt()
    {
        var childBook = CreateChildBook();
        var recommendation = new Recommendation("Some recommendation", AdminId, Guid.NewGuid());
        typeof(Recommendation).GetProperty(nameof(Recommendation.ChildBooks))!.SetValue(recommendation, new List<ChildBook>());

        childBook.AddRecommendationsIfNeeded(new List<Recommendation> { recommendation });

        Assert.Single(childBook.Recommendations);
    }

    [Fact]
    public void AddRecommendationsIfNeeded_SameRecommendationTwice_ShouldNotDuplicate()
    {
        var childBook = CreateChildBook();
        var recommendation = new Recommendation("Some recommendation", AdminId, Guid.NewGuid());
        typeof(Recommendation).GetProperty(nameof(Recommendation.ChildBooks))!.SetValue(recommendation, new List<ChildBook>());

        childBook.AddRecommendationsIfNeeded(new List<Recommendation> { recommendation });
        childBook.AddRecommendationsIfNeeded(new List<Recommendation> { recommendation });

        Assert.Single(childBook.Recommendations);
    }
}
