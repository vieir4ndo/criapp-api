namespace CRIApp.Domain.Entities;

public class RecommendationCheckpoint
{
    public Guid Id { get; private set; }
    public int RecommendationId { get; private set; }
    public Guid ChildBookId { get; private set; }
    public DateTime Date { get; private set; }
    public bool Completed { get; private set; }

    public Recommendation? Recommendation { get; private set; }
    public ChildBook? ChildBook { get; private set; }

    protected RecommendationCheckpoint() { }

    public RecommendationCheckpoint(int recommendationId, Guid childBookId, bool completed)
    {
        Id = Guid.NewGuid();
        RecommendationId = recommendationId;
        ChildBookId = childBookId;
        Date = DateTime.UtcNow.Date;
        Completed = completed;
    }
}
