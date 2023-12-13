using CRIApp.Domain.Entities;

namespace CRIApp.Application.Dto;

public class CreateAnswerDto
{
    public int Value { get; set; }
    public Guid QuestionId { get; set; }
    public Guid ChildId { get; set; }
    public Guid PrcId { get; set; }

    public Answer ToDomainObject()
    {
        return new Answer(Value, QuestionId, ChildId, PrcId);
    }
}