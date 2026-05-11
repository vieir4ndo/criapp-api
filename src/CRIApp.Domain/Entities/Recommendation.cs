using CRIApp.Domain.Extensions;

namespace CRIApp.Domain.Entities;

public class Recommendation
{
    public int Id { get; set; }
    public string Text { get; private set; }
    public string? TextEn { get; private set; }
    public string? TextPt { get; private set; }
    public Question Question { get; private set; }
    public Guid QuestionId { get; private set; }
    public byte[]? Image { get; private set; }
    public List<ChildBook> ChildBooks { get; private set; }
    public int ReferenceNumber { get; private set; }

    protected Recommendation()
    {
    }

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

    public Recommendation WithImage(byte[]? image)
    {
        Image = image;
        return this;
    }

    private void Validate()
    {
        Text.IsNullOrEmpty("A recommendation cannot have a null or empty text");
        QuestionId.IsEmptyGuid("A recommendation has to have a question associated");
    }
}