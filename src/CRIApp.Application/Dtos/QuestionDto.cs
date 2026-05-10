using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using System.Globalization;

namespace CRIApp.Application.Dtos;

public class QuestionDto
{
    public Guid Id { get; set; }    
    public string Text { get; set; }
    public bool Active { get; set; }
    public QuestionType Type { get; set; }
    public QuestionScoringType ScoringType { get; set; }

    public QuestionDto(Question question)
    {
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        Text = lang switch
        {
            "en" => question.TextEn ?? question.Text,
            "pt" => question.TextPt ?? question.Text,
            _ => question.Text
        };
        Active = question.Active;
        Id = question.Id;
        Type = question.Type;
        ScoringType = question.ScoringType;
    }
}