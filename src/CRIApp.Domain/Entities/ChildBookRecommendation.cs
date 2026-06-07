namespace CRIApp.Domain.Entities;

public class ChildBookRecommendation
{
    public Guid ChildBooksId { get; set; }
    public int RecommendationsId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ChildBook? ChildBook { get; set; }
    public Recommendation? Recommendation { get; set; }
}
