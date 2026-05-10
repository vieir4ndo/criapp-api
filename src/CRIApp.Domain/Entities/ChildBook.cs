using CRIApp.Domain.Enums;

namespace CRIApp.Domain.Entities;

public class ChildBook : Entity
{
    public Guid ChildId { get; private set; }
    public Child Child { get; private set; } = null!;
    public int Score { get; private set; }
    public DateTime ScoreLastUpdatedAt { get; private set; }
    public List<Award> Awards { get; private set; } = new();
    public List<Advice> Advices { get; private set; } = new();

    protected ChildBook() { }

    public ChildBook(Guid childId)
    {
        ChildId = childId;
        Score = 0;
        ScoreLastUpdatedAt = DateTime.UtcNow;
    }

    public void UpdateScore(List<Answer> answers)
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

        Score = points * 100 / answers.Count;
        ScoreLastUpdatedAt = DateTime.UtcNow;
    }

    public void AddAwardIfNeeded(Award award)
    {
        if (Awards.All(a => a.Type != award.Type))
            Awards.Add(award);
    }

    public void AddAdvicesIfNeeded(IList<Advice> advices)
    {
        foreach (var advice in advices)
        {
            if (!Advices.Contains(advice))
                Advices.Add(advice);
        }
    }
}
