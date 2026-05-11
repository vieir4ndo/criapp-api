using CRIApp.Domain.Entities;

namespace CRIApp.Application.Dtos;

public class ChildBookSummaryDto
{
    public Guid Id { get; set; }
    public Guid ChildId { get; set; }
    public int Score { get; set; }
    public DateTime ScoreLastUpdatedAt { get; set; }

    public ChildBookSummaryDto(ChildBook childBook)
    {
        Id = childBook.Id;
        ChildId = childBook.ChildId;
        Score = childBook.Score;
        ScoreLastUpdatedAt = childBook.ScoreLastUpdatedAt;
    }
}
