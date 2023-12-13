using CRIApp.Domain.Enums;
using CRIApp.Domain.Extensions;

namespace CRIApp.Domain.Entities;

public class Question : Entity
{
    public string Text { get; private set; }
    public bool Active { get; private set; }
    public QuestionType Type { get; private set; }
    public IList<Answer> Answers { get; private set; }
    public Guid AdminId { get; private set; }
    public User Admin { get; private set; }

    protected Question() {}
    
    public Question(string text, Guid adminId, QuestionType type)
    {
        Text = text;
        AdminId = adminId;
        Answers = new List<Answer>();
        Active = true;
        Type = type;
        
        Validate();
    }

    private void Validate()
    {
        Text.IsNullOrEmpty("A question cannot have a null or empty text");
        AdminId.IsEmptyGuid("A question has to have an administrator associated");
        Answers.HasElements("A question that has been answered cannot be written");
        Active.IsActive("A question has to be active to be written");
        Type.IsInEnum("A question has to have a valid type");
    }

    public void Activate() => Active = true;
    public void Deactivate() => Active = false;
    public void ChangeText(string text)
    {
        Text = text;
        
        Validate();
    }
}