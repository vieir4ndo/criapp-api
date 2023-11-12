using CRIApp.Domain.Entities;

namespace CRIApp.Application.Dtos;

public class QuestionDto
{
    public string Text { get; set; }
    public bool Active { get; set; }
    public Guid AdminId { get; set; }

    public QuestionDto(Question question)
    {
        Text = question.Text;
        Active = question.Active;
        AdminId = question.AdminId;
    }
}