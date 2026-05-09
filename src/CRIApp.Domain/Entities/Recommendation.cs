using CRIApp.Domain.Extensions;

namespace CRIApp.Domain.Entities;

public class Recommendation
{
    public int Id { get; set; }
    public string Text { get; private set; }
    public string? TextEn { get; private set; }
    public string? TextPt { get; private set; }
    public Guid AdminId { get; private set; }
    public User Admin { get; private set; }
    public Question Question { get; private set; }
    public Guid QuestionId { get; private set; }
    public List<User> Users { get; private set; }
    public int ReferenceNumber { get; private set; }

    protected Recommendation()
    {
    }

    public Recommendation(string text, Guid adminId, Guid questionId)
    {
        Text = text;
        AdminId = adminId;
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
        AdminId.IsEmptyGuid("A recommendation has to have an administrator associated");
        QuestionId.IsEmptyGuid("A recommendation has to have an question associated");
    }
}