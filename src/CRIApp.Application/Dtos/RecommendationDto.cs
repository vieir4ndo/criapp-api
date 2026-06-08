using CRIApp.Domain.Entities;
using System.Globalization;

namespace CRIApp.Application.Dtos;

public class RecommendationDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public Guid QuestionId { get; set; }
    public string? Image { get; set; }
    public bool? TodayCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CompletedCheckpoints { get; set; }

    public RecommendationDto(Recommendation recommendation, bool? todayCompleted = null, DateTime assignedAt = default, int completedCheckpoints = 0)
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
        Image = recommendation.Image != null ? Convert.ToBase64String(recommendation.Image) : null;
        TodayCompleted = todayCompleted;
        CreatedAt = assignedAt;
        CompletedCheckpoints = completedCheckpoints;
    }
}