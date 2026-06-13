using CRIApp.Domain.Entities;
using System.Globalization;

namespace CRIApp.Application.Dtos;

public class RecommendationDto
{
    public int Id { get; set; }
    public Guid ChildBookRecommendationId { get; set; }
    public string Text { get; set; }
    public Guid QuestionId { get; set; }
    public string? Image { get; set; }
    public bool? IsCompletedToday { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CompletedCountInWindow { get; set; }

    public RecommendationDto(ChildBookRecommendation cbr)
    {
        var rec = cbr.Recommendation!;
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        Text = lang switch
        {
            "en" => rec.TextEn ?? rec.Text,
            "pt" => rec.TextPt ?? rec.Text,
            _ => rec.Text
        };
        Id = rec.Id;
        ChildBookRecommendationId = cbr.Id;
        QuestionId = rec.QuestionId;
        Image = rec.Image != null ? Convert.ToBase64String(rec.Image) : null;
        IsCompletedToday = cbr.IsCompletedToday();
        CreatedAt = DateTime.SpecifyKind(cbr.CreatedAt, DateTimeKind.Utc);
        CompletedCountInWindow = cbr.GetCompletedCountInWindow();
    }
}