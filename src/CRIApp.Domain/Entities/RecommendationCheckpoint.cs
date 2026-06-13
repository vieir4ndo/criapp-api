namespace CRIApp.Domain.Entities;

public class RecommendationCheckpoint
{
    public Guid Id { get; private set; }
    public Guid ChildBookRecommendationId { get; private set; }
    public Guid ChildBookId { get; private set; }
    public DateTime Date { get; private set; }
    public bool Completed { get; private set; }

    public ChildBookRecommendation? ChildBookRecommendation { get; private set; }
    public ChildBook? ChildBook { get; private set; }

    protected RecommendationCheckpoint() { }

    public RecommendationCheckpoint(Guid childBookRecommendationId, Guid childBookId, bool completed)
    {
        Id = Guid.NewGuid();
        ChildBookRecommendationId = childBookRecommendationId;
        ChildBookId = childBookId;
        Date = DateTime.UtcNow.Date;
        Completed = completed;
    }
}
