
using CRIApp.Domain.Enums;
using CRIApp.Domain.Extensions;

namespace CRIApp.Domain.Entities;

public class Answer : Entity
{
    public int Value { get; private set; }
    
    public Guid QuestionId { get; private set; }
    public Question Question { get; private set; } = null!;

    public Guid ChildId { get; private set; }
    public Child Child { get; private set; } = null!;

    public Guid PrcId { get; private set; }
    public User Prc { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    public Answer(int value, Guid questionId, Guid childId, Guid prcId)
    {
        Value = value;
        QuestionId = questionId;
        ChildId = childId;
        PrcId = prcId;
        CreatedAt = DateTime.Now.ToUniversalTime();
        
        Validate();
    }

    private void Validate()
    {
        Value.IsValueInRange(1, 6, "Answer is out of range");
        QuestionId.IsEmptyGuid("An answer has to refer to a question");
        ChildId.IsEmptyGuid("An answer has to have a Child associated");
        PrcId.IsEmptyGuid("An answer has to have a Prc associated");
    }

    public bool IsCorrect()
    {
        List<int> correctAnswers = Question.ScoringType switch
        {
            QuestionScoringType.Default => [5, 6],
            QuestionScoringType.Reversed => [1, 2],
            _ => throw new ArgumentOutOfRangeException()
        };

        return correctAnswers.Contains(Value);
    }
}