using CRIApp.Domain.Entities;

namespace CRIApp.Application.Dtos;

public class RecommendationDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public Guid QuestionId { get; set; }

    public RecommendationDto(Recommendation recommendation)
    {
        Id = recommendation.Id;
        Text = recommendation.Text;
        QuestionId = recommendation.QuestionId;
    }
}
