namespace CRIApp.Domain.Entities;

public class UserRecommendation
{
    public Guid UsersId { get; set; }
    public int RecommendationsId { get; set; }

    public User? User { get; set; }
    public Recommendation? Recommendation { get; set; }
}

