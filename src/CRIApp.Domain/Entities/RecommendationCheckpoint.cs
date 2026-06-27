namespace CRIApp.Domain.Entities;
using Extensions;

public class RecommendationCheckpoint : Entity
{
    public Guid ChildBookRecommendationId { get; private set; }
    public DateTime Date { get; private set; }
    public bool Completed { get; private set; }

    public ChildBookRecommendation? ChildBookRecommendation { get; private set; }

    public RecommendationCheckpoint(Guid childBookRecommendationId, bool completed)
    {
        ChildBookRecommendationId = childBookRecommendationId;
        Date = DateTime.UtcNow.Date;
        Completed = completed;

        Validate();
    }

    private void Validate()
    {
        ChildBookRecommendationId.IsEmptyGuid("A checkpoint has to have a child book recommendation associated");
    }
}
