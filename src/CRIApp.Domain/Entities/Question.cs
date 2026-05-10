using CRIApp.Domain.Enums;
using CRIApp.Domain.Extensions;

namespace CRIApp.Domain.Entities;

public class Question : Entity
{
    public string Text { get; private set; }
    public string? TextEn { get; private set; }
    public string? TextPt { get; private set; }
    public bool Active { get; private set; }
    public QuestionType Type { get; private set; }
    public IList<Answer> Answers { get; private set; }
    public IList<FamilyType> ExcludedFamilyTypes { get; private set; }
    public QuestionScoringType ScoringType { get; private set; }
    public QuestionGroup Group { get; private set; }
    public IList<Recommendation> Recommendations { get; private set; } = new List<Recommendation>();

    protected Question() {}
    
    public Question(string text, Guid adminId, QuestionType type, QuestionScoringType scoringType = QuestionScoringType.Default, IList<FamilyType>? excludedFamilyTypes = null, QuestionGroup group = QuestionGroup.CognitiveDevelopment)
    {
        Text = text;
        Answers = new List<Answer>();
        Active = true;
        Type = type;
        ScoringType = scoringType;
        ExcludedFamilyTypes = excludedFamilyTypes ?? new List<FamilyType>();
        Group = group;
        
        Validate();
    }

    public Question WithTranslations(string? textEn, string? textPt)
    {
        TextEn = textEn;
        TextPt = textPt;
        return this;
    }

    private void Validate()
    {
        Text.IsNullOrEmpty("A question cannot have a null or empty text");
        Answers.HasElements("A question that has been answered cannot be written");
        Active.IsActive("A question has to be active to be written");
        Type.IsInEnum("A question has to have a valid type");
        ScoringType.IsInEnum("A question has to have a valid scoring type");
    }

    public void Activate() => Active = true;
    public void Deactivate() => Active = false;
    public void ChangeText(string text)
    {
        Text = text;
        
        Validate();
    }
}