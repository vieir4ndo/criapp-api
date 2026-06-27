using CRIApp.Domain.Extensions;

namespace CRIApp.Domain.Entities;

public class Recommendation
{
    public int Id { get; set; }
    public string Text { get; private set; }
    public string? TextEn { get; private set; }
    public string? TextPt { get; private set; }
    public Question Question { get; private set; } = null!;
    public Guid QuestionId { get; private set; }
    public List<ChildBook> ChildBooks { get; private set; } = null!;

    public Recommendation(string text, Guid questionId)
    {
        Text = text;
        QuestionId = questionId;

        Validate();
    }

    public Recommendation WithTranslations(string? textEn, string? textPt)
    {
        TextEn = textEn;
        TextPt = textPt;
        return this;
    }

    private void Validate()
    {
        Text.IsNullOrEmpty("A recommendation cannot have a null or empty text");
        QuestionId.IsEmptyGuid("A recommendation has to have a question associated");
    }
}