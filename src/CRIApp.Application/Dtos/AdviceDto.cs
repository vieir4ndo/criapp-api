using CRIApp.Domain.Entities;
using System.Globalization;

namespace CRIApp.Application.Dtos;

public class AdviceDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public Guid QuestionId { get; set; }

    public AdviceDto(Advice advice)
    {
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        Text = lang switch
        {
            "en" => advice.TextEn ?? advice.Text,
            "pt" => advice.TextPt ?? advice.Text,
            _ => advice.Text
        };
        Id = advice.Id;
        QuestionId = advice.QuestionId;
    }
}