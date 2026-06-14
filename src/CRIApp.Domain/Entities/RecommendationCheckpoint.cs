namespace CRIApp.Domain.Entities;

public class RecommendationCheckpoint
{
    public Guid Id { get; private set; }
    public Guid ChildBookRecommendationId { get; private set; }
    public DateTime Date { get; private set; }
    public bool Completed { get; private set; }

    public ChildBookRecommendation? ChildBookRecommendation { get; private set; }

    protected RecommendationCheckpoint() { }

    public RecommendationCheckpoint(Guid childBookRecommendationId, bool completed)
    {
        Id = Guid.NewGuid();
        ChildBookRecommendationId = childBookRecommendationId;
        Date = DateTime.UtcNow.Date;
        Completed = completed;
    }
}
