using CRIApp.Domain.Enums;

namespace CRIApp.Domain.Entities;

public class ChildBook : Entity
{
    public Guid ChildId { get; private set; }
    public Child Child { get; private set; } = null!;
    public int Score { get; private set; }
    public DateTime ScoreLastUpdatedAt { get; private set; }
    public List<Award> Awards { get; private set; } = new();
    public List<Recommendation> Recommendations { get; private set; } = new();

    protected ChildBook() { }

    public ChildBook(Guid childId)
    {
        ChildId = childId;
        Score = 0;
        ScoreLastUpdatedAt = DateTime.UtcNow;
    }

    public void UpdateScore(List<Answer> answers, List<RecommendationCheckpoint> todayCheckpoints)
    {
        if (!answers.Any())
        {
            Score = 0;
            ScoreLastUpdatedAt = DateTime.UtcNow;
            return;
        }

        var points = 0;
        foreach (var answer in answers)
        {
            List<int> values = answer.Question.ScoringType switch
            {
                QuestionScoringType.Default => [1, 2, 3, 4],
                QuestionScoringType.Reversed => [5, 6],
                _ => throw new ArgumentOutOfRangeException()
            };

            if (values.Contains(answer.Value))
                points++;
        }

        var answersRatio = (double)points / answers.Count;
        var totalActiveRecommendations = Recommendations.Count;

        if (totalActiveRecommendations == 0)
        {
            Score = (int)Math.Round(answersRatio * 100);
        }
        else
        {
            var completedToday = todayCheckpoints.Count(c => c.Completed);
            var checkpointRatio = (double)completedToday / totalActiveRecommendations;
            Score = (int)Math.Round(answersRatio * 70 + checkpointRatio * 30);
        }

        ScoreLastUpdatedAt = DateTime.UtcNow;
    }

    public void AddAwardIfNeeded(Award award)
    {
        if (Awards.All(a => a.Type != award.Type))
            Awards.Add(award);
    }

    public void AddRecommendationsIfNeeded(IList<Recommendation> recommendations)
    {
        foreach (var recommendation in recommendations)
        {
            if (!Recommendations.Contains(recommendation))
                Recommendations.Add(recommendation);
        }
    }

    public void RemoveRecommendation(int recommendationId)
    {
        var recommendation = Recommendations.FirstOrDefault(r => r.Id == recommendationId);
        if (recommendation is not null)
            Recommendations.Remove(recommendation);
    }
}
