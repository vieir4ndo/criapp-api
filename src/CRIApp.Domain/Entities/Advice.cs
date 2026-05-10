using CRIApp.Domain.Extensions;

namespace CRIApp.Domain.Entities;

public class Advice
{
    public int Id { get; set; }
    public string Text { get; private set; }
    public string? TextEn { get; private set; }
    public string? TextPt { get; private set; }
    public Question Question { get; private set; }
    public Guid QuestionId { get; private set; }
    public List<User> Users { get; private set; }
    public int ReferenceNumber { get; private set; }

    protected Advice()
    {
    }

    public Advice(string text, Guid adminId, Guid questionId)
    {
        Text = text;
        QuestionId = questionId;

        Validate();
    }

    public Advice WithTranslations(string? textEn, string? textPt)
    {
        TextEn = textEn;
        TextPt = textPt;
        return this;
    }

    private void Validate()
    {
        Text.IsNullOrEmpty("An advice cannot have a null or empty text");
        QuestionId.IsEmptyGuid("An advice has to have a question associated");
    }
}