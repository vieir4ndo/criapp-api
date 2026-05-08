namespace CRIApp.Domain.Entities;
using Extensions;

public class Faq : Entity
{
    public string Question { get; private set; }
    public string Answer { get; private set; }
    public string? QuestionEn { get; private set; }
    public string? AnswerEn { get; private set; }
    public string? QuestionPt { get; private set; }
    public string? AnswerPt { get; private set; }
    public Guid AdminId { get; private set; }
    public User Admin { get; private set; }
    
    public Faq(string question, string answer, Guid adminId)
    {
        Question = question;
        Answer = answer;
        AdminId = adminId;
        
        Validate();
    }

    public Faq WithTranslations(string? questionEn, string? answerEn, string? questionPt, string? answerPt)
    {
        QuestionEn = questionEn;
        AnswerEn = answerEn;
        QuestionPt = questionPt;
        AnswerPt = answerPt;
        return this;
    }

    private void Validate()
    {
        Question.IsNullOrEmpty("A faq cannot have a null or empty question");
        Answer.IsNullOrEmpty("A faq cannot have a null or empty answer");
        AdminId.IsEmptyGuid("A faq has to have an administrator associated");
    }
}