namespace CRIApp.Domain.Entities;

public class ChildBookRecommendation
{
    private const int AssimilationWindowDays = 7;
    private const int AssimilationThreshold = 3;

    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ChildBooksId { get; set; }
    public int RecommendationId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ChildBook? ChildBook { get; set; }
    public Recommendation? Recommendation { get; set; }
    public IList<RecommendationCheckpoint> Checkpoints { get; private set; } = new List<RecommendationCheckpoint>();

    public bool IsAssimilated()
    {
        var windowStart = DateTime.UtcNow.Date.AddDays(-(AssimilationWindowDays - 1));
        return Checkpoints
            .Where(c => c.Completed && c.Date >= windowStart)
            .Select(c => c.Date)
            .Distinct()
            .Count() >= AssimilationThreshold;
    }

    public bool? IsCompletedToday()
    {
        var today = DateTime.UtcNow.Date;
        return Checkpoints.FirstOrDefault(c => c.Date == today)?.Completed;
    }

    public int GetCompletedCountInWindow()
    {
        var windowStart = DateTime.UtcNow.Date.AddDays(-(AssimilationWindowDays - 1));
        return Checkpoints
            .Where(c => c.Completed && c.Date >= windowStart)
            .Select(c => c.Date)
            .Distinct()
            .Count();
    }
}
