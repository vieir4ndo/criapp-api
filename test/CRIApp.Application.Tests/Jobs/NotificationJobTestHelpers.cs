using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Application.Tests.Jobs;

public static class NotificationJobTestHelpers
{
    private static readonly DateTime Today = DateTime.UtcNow.Date;

    public static ChildBook BuildChildBook(
        RecommendationCheckpoint? checkpoint = null,
        Answer? answer = null)
    {
        var prcId = Guid.NewGuid();
        var child = new Child("Child", new DateTime(2020, 1, 1), Sex.Male, 6, prcId, false);
        var answers = answer is not null ? new List<Answer> { answer } : new List<Answer>();
        typeof(Child).GetProperty(nameof(Child.Answers))!.SetValue(child, answers);

        var childBook = new ChildBook(child.Id);
        typeof(ChildBook).GetProperty(nameof(ChildBook.Child))!.SetValue(childBook, child);

        var cbr = new ChildBookRecommendation();
        if (checkpoint is not null)
            cbr.Checkpoints.Add(checkpoint);
        childBook.ChildBookRecommendations.Add(cbr);

        return childBook;
    }

    public static RecommendationCheckpoint BuildCheckpoint(DateTime date)
    {
        var cp = new RecommendationCheckpoint(Guid.NewGuid(), true);
        typeof(RecommendationCheckpoint)
            .GetProperty(nameof(RecommendationCheckpoint.Date),
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .GetSetMethod(nonPublic: true)!
            .Invoke(cp, new object[] { date });
        return cp;
    }

    public static Answer BuildAnswer(DateTime createdAt)
    {
        var answer = new Answer(1, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        typeof(Answer)
            .GetProperty(nameof(Answer.CreatedAt),
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
            .SetValue(answer, createdAt);
        return answer;
    }
}
