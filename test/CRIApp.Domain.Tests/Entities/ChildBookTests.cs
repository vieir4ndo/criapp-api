using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class ChildBookTests
{
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
        var question = new Question("Sample question?", QuestionType.Agreement, scoringType);
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

        childBook.UpdateScore(childBook.Child.Answers);

        Assert.Equal(0, childBook.Score);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(6)]
    public void UpdateScore_DefaultScoringWithCountableValue_ShouldScore100(int value)
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(value, QuestionScoringType.Default));

        childBook.UpdateScore(childBook.Child.Answers);

        Assert.Equal(100, childBook.Score);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void UpdateScore_DefaultScoringWithLowValue_ShouldNotCount(int value)
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(value, QuestionScoringType.Default));

        childBook.UpdateScore(childBook.Child.Answers);

        Assert.Equal(0, childBook.Score);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void UpdateScore_ReversedScoringWithLowValue_ShouldScore100(int value)
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(value, QuestionScoringType.Reversed));

        childBook.UpdateScore(childBook.Child.Answers);

        Assert.Equal(100, childBook.Score);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public void UpdateScore_ReversedScoringWithHighValue_ShouldNotCount(int value)
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(value, QuestionScoringType.Reversed));

        childBook.UpdateScore(childBook.Child.Answers);

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

        childBook.UpdateScore(childBook.Child.Answers);

        Assert.Equal(50, childBook.Score);
    }

    [Fact]
    public void UpdateScore_ShouldUpdateScoreLastUpdatedAt()
    {
        var childBook = CreateChildBook();
        var before = DateTime.UtcNow;

        childBook.UpdateScore(childBook.Child.Answers);

        Assert.True(childBook.ScoreLastUpdatedAt >= before);
    }

    private static ChildBookRecommendation BuildChildBookRecommendation(ChildBook childBook, Recommendation rec)
    {
        var cbr = new ChildBookRecommendation
        {
            ChildBooksId = childBook.Id,
            RecommendationsId = rec.Id,
            Recommendation = rec
        };
        return cbr;
    }

    private static RecommendationCheckpoint BuildCheckpoint(Guid childBookRecommendationId, Guid childBookId, bool completed)
        => new(childBookRecommendationId, childBookId, completed);

    [Fact]
    public void UpdateScore_NoRecommendations_ShouldApplyFullAnswersWeight()
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(5, QuestionScoringType.Default));

        childBook.UpdateScore(childBook.Child.Answers);

        Assert.Equal(100, childBook.Score);
    }

    [Fact]
    public void UpdateScore_WithRecommendationsAndNoCheckpoints_ShouldApply70AnswersWeight()
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(5, QuestionScoringType.Default));
        var rec = new Recommendation("Recommendation", Guid.NewGuid());
        childBook.AddRecommendationsIfNeeded([rec]);
        var cbr = BuildChildBookRecommendation(childBook, rec);
        childBook.ChildBookRecommendations.Add(cbr);

        childBook.UpdateScore(childBook.Child.Answers);

        Assert.Equal(70, childBook.Score);
    }

    [Fact]
    public void UpdateScore_WithAllCheckpointsCompleted_ShouldScore100()
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(5, QuestionScoringType.Default));
        var rec = new Recommendation("Recommendation", Guid.NewGuid());
        childBook.AddRecommendationsIfNeeded([rec]);
        var cbr = BuildChildBookRecommendation(childBook, rec);
        cbr.Checkpoints.Add(BuildCheckpoint(cbr.Id, childBook.Id, completed: true));
        childBook.ChildBookRecommendations.Add(cbr);

        childBook.UpdateScore(childBook.Child.Answers);

        Assert.Equal(100, childBook.Score);
    }

    [Fact]
    public void UpdateScore_WithPartialCheckpoints_ShouldApplyPartialCheckpointWeight()
    {
        var childBook = CreateChildBook();
        childBook.Child.Answers.Add(BuildAnswerWithQuestion(5, QuestionScoringType.Default));
        var rec1 = new Recommendation("Rec1", Guid.NewGuid());
        var rec2 = new Recommendation("Rec2", Guid.NewGuid());
        childBook.AddRecommendationsIfNeeded([rec1, rec2]);
        var cbr1 = BuildChildBookRecommendation(childBook, rec1);
        var cbr2 = BuildChildBookRecommendation(childBook, rec2);
        cbr1.Checkpoints.Add(BuildCheckpoint(cbr1.Id, childBook.Id, completed: true));
        cbr2.Checkpoints.Add(BuildCheckpoint(cbr2.Id, childBook.Id, completed: false));
        childBook.ChildBookRecommendations.Add(cbr1);
        childBook.ChildBookRecommendations.Add(cbr2);

        childBook.UpdateScore(childBook.Child.Answers);

        Assert.Equal(85, childBook.Score);
    }

    [Fact]
    public void AddAwardIfNeeded_NewAwardType_ShouldAddAward()
    {
        var childBook = CreateChildBook();
        var award = new Award(AwardType.HabitBuilder, "Habit Builder");

        childBook.AddAwardIfNeeded(award);

        Assert.Single(childBook.Awards);
        Assert.Equal(AwardType.HabitBuilder, childBook.Awards[0].Type);
    }

    [Fact]
    public void AddAwardIfNeeded_SameAwardTypeTwice_ShouldNotDuplicate()
    {
        var childBook = CreateChildBook();
        var award = new Award(AwardType.HabitBuilder, "Habit Builder");

        childBook.AddAwardIfNeeded(award);
        childBook.AddAwardIfNeeded(award);

        Assert.Single(childBook.Awards);
    }

    [Fact]
    public void AddAwardIfNeeded_DifferentAwardTypes_ShouldAddBoth()
    {
        var childBook = CreateChildBook();

        childBook.AddAwardIfNeeded(new Award(AwardType.HabitBuilder, "Habit Builder"));
        childBook.AddAwardIfNeeded(new Award(AwardType.ConsistentParent, "Consistent Parent"));

        Assert.Equal(2, childBook.Awards.Count);
    }

    [Fact]
    public void AddAwardIfNeeded_AllSevenTypes_ShouldAddAllSeven()
    {
        var childBook = CreateChildBook();

        childBook.AddAwardIfNeeded(new Award(AwardType.HabitBuilder, "Habit Builder"));
        childBook.AddAwardIfNeeded(new Award(AwardType.ConsistentParent, "Consistent Parent"));
        childBook.AddAwardIfNeeded(new Award(AwardType.SilverCognitiveDevelopment, "Silver Cognitive"));
        childBook.AddAwardIfNeeded(new Award(AwardType.SilverEmotionalBonding, "Silver Emotional"));
        childBook.AddAwardIfNeeded(new Award(AwardType.SilverDailyRoutines, "Silver Routines"));
        childBook.AddAwardIfNeeded(new Award(AwardType.SilverSocialInteraction, "Silver Social"));
        childBook.AddAwardIfNeeded(new Award(AwardType.GoldParent, "Gold Parent"));

        Assert.Equal(7, childBook.Awards.Count);
    }

    [Fact]
    public void AddRecommendationsIfNeeded_NewRecommendation_ShouldAddIt()
    {
        var childBook = CreateChildBook();
        var recommendation = new Recommendation("Some recommendation", Guid.NewGuid());
        typeof(Recommendation).GetProperty(nameof(Recommendation.ChildBooks))!.SetValue(recommendation, new List<ChildBook>());

        childBook.AddRecommendationsIfNeeded(new List<Recommendation> { recommendation });

        Assert.Single(childBook.Recommendations);
    }

    [Fact]
    public void AddRecommendationsIfNeeded_SameRecommendationTwice_ShouldNotDuplicate()
    {
        var childBook = CreateChildBook();
        var recommendation = new Recommendation("Some recommendation", Guid.NewGuid());
        typeof(Recommendation).GetProperty(nameof(Recommendation.ChildBooks))!.SetValue(recommendation, new List<ChildBook>());

        childBook.AddRecommendationsIfNeeded(new List<Recommendation> { recommendation });
        childBook.AddRecommendationsIfNeeded(new List<Recommendation> { recommendation });

        Assert.Single(childBook.Recommendations);
    }
}
