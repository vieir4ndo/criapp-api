using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Application.Dtos;

public class QuestionDto
{
    public Guid Id { get; set; }    
    public Guid AdminId { get; set; }
    public string Text { get; set; }
    public bool Active { get; set; }
    public QuestionType Type { get; set; }

    public QuestionDto(Question question)
    {
        Text = question.Text;
        Active = question.Active;
        Id = question.Id;
        AdminId = question.AdminId;
        Type = question.Type;
    }
}