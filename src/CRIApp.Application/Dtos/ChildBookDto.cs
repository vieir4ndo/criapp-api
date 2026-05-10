using CRIApp.Domain.Entities;

namespace CRIApp.Application.Dtos;

public class ChildBookDto
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public int Score { get; set; }
    public DateTime ScoreLastUpdatedAt { get; set; }
    public List<AwardDto> Awards { get; set; }
    public List<RecommendationDto> Recommendations { get; set; }

    public ChildBookDto(ChildBook childBook, IList<RecommendationCheckpoint>? todayCheckpoints = null)
    {
        Id = childBook.Id;
        ChildId = childBook.ChildId;
        Score = childBook.Score;
        ScoreLastUpdatedAt = childBook.ScoreLastUpdatedAt;
        Awards = childBook.Awards.Select(a => new AwardDto(a)).ToList();

        var checkpointMap = todayCheckpoints?.ToDictionary(c => c.RecommendationId, c => (bool?)c.Completed);
        Recommendations = childBook.Recommendations.Select(r =>
        {
            bool? todayCompleted = null;
            checkpointMap?.TryGetValue(r.Id, out todayCompleted);
            return new RecommendationDto(r, todayCompleted);
        }).ToList();
    }
}
