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

    public ChildBookDto(ChildBook childBook)
    {
        Id = childBook.Id;
        ChildId = childBook.ChildId;
        Score = childBook.Score;
        ScoreLastUpdatedAt = childBook.ScoreLastUpdatedAt;
        Awards = childBook.Awards.Select(a => new AwardDto(a)).ToList();
        Recommendations = childBook.Recommendations.Select(r => new RecommendationDto(r)).ToList();
    }
}
