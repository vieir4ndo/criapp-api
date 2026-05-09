using CRIApp.Domain.Entities;
using System.Globalization;

namespace CRIApp.Application.Dtos;

public class RecommendationDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public Guid QuestionId { get; set; }

    public RecommendationDto(Recommendation recommendation)
    {
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        Text = lang switch
        {
            "en" => recommendation.TextEn ?? recommendation.Text,
            "pt" => recommendation.TextPt ?? recommendation.Text,
            _ => recommendation.Text
        };
        Id = recommendation.Id;
        QuestionId = recommendation.QuestionId;
    }
}
