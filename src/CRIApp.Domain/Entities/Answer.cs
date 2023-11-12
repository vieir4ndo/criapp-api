
namespace CRIApp.Domain.Entities;

public class Answer : Entity
{
    public int Value { get; private set; }
    
    public Guid QuestionId { get; private set; }
    public Question Question { get; private set; }
    
    public Guid ChildId { get; private set; }
    public Child Child { get; private set; }
    
    public Guid PrcId { get; private set; }
    public User Prc { get; private set; }

    protected Answer() {}

    public Answer(int value, Guid questionId, Guid childId, Guid prcId)
    {
        Value = value;
        QuestionId = questionId;
        ChildId = childId;
        PrcId = prcId;
        
        Validate();
    }

    private void Validate()
    {
        Value.IsValueInRange(0,5, "Answer is out of range");
        QuestionId.IsEmptyGuid("An answer has to refer to a question");
        PrcId.IsEmptyGuid("An answer has to have a Prc associated");
        PrcId.IsEmptyGuid("An answer has to have a Child associated");
    }
}