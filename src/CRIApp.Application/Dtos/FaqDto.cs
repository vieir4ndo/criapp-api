using CRIApp.Domain.Entities;

namespace CRIApp.Application.Dtos;

public class FaqDto
{
    public Guid Id { get; set; }
    public string Question { get; private set; }
    public string Answer { get; private set; }

    public FaqDto(Faq faq)
    {
        Id = faq.Id;
        Question = faq.Question;
        Answer = faq.Answer;
    }
}