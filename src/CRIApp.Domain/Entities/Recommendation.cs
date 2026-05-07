using CRIApp.Domain.Extensions;

namespace CRIApp.Domain.Entities;

public class Recommendation : Entity
{
    public string Text { get; private set; }
    public Guid AdminId { get; private set; }
    public User Admin { get; private set; }
    public Question Question { get; private set; }
    public Guid QuestionId { get; private set; }

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

    private void Validate()
    {
        Text.IsNullOrEmpty("A recommendation cannot have a null or empty text");
        AdminId.IsEmptyGuid("A recommendation has to have an administrator associated");
        QuestionId.IsEmptyGuid("A recommendation has to have an question associated");
    }
}