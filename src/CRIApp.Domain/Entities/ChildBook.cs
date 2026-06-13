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
    public List<RecommendationCheckpoint> RecommendationCheckpoints { get; private set; } = new();
    public List<ChildBookRecommendation> ChildBookRecommendations { get; private set; } = new();

    protected ChildBook() { }

    public ChildBook(Guid childId)
    {
        ChildId = childId;
        Score = 0;
        ScoreLastUpdatedAt = DateTime.UtcNow;
    }

    public void UpdateScore(List<Answer> answers)
    {
        if (answers.Count == 0)
        {
            Score = 0;
            ScoreLastUpdatedAt = DateTime.UtcNow;
            return;
        }

        var points = answers.Count(answer => answer.IsCorrect());

        var answersRatio = (double)points / answers.Count;
        var totalActiveRecommendations = ChildBookRecommendations.Count;

        if (totalActiveRecommendations == 0)
        {
            Score = (int)Math.Round(answersRatio * 100);
        }
        else
        {
            var completedToday = ChildBookRecommendations.Count(cbr => cbr.IsCompletedToday() == true);
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
