using CRIApp.Domain.Entities;
using System.Globalization;

namespace CRIApp.Application.Dtos;

public class FaqDto
{
    public Guid Id { get; set; }
    public string Question { get; private set; }
    public string Answer { get; private set; }

    public FaqDto(Faq faq)
    {
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        Id = faq.Id;
        Question = lang switch
        {
            "en" => faq.QuestionEn ?? faq.Question,
            "pt" => faq.QuestionPt ?? faq.Question,
            _ => faq.Question
        };
        Answer = lang switch
        {
            "en" => faq.AnswerEn ?? faq.Answer,
            "pt" => faq.AnswerPt ?? faq.Answer,
            _ => faq.Answer
        };
    }
}