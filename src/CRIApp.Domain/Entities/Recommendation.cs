using CRIApp.Domain.Extensions;

namespace CRIApp.Domain.Entities;

public class Recommendation
{
    private const int AssimilationWindowDays = 7;
    private const int AssimilationThreshold = 4;
    public int Id { get; set; }
    public string Text { get; private set; }
    public string? TextEn { get; private set; }
    public string? TextPt { get; private set; }
    public Question Question { get; private set; }
    public Guid QuestionId { get; private set; }
    public byte[]? Image { get; private set; }
    public List<ChildBook> ChildBooks { get; private set; }
    public IList<RecommendationCheckpoint> Checkpoints { get; private set; } = new List<RecommendationCheckpoint>();

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

    public bool IsAssimilated()
    {
        var windowStart = DateTime.UtcNow.Date.AddDays(-(AssimilationWindowDays - 1));
        var distinctCompletedDays = Checkpoints
            .Where(c => c.Completed && c.Date >= windowStart)
            .Select(c => c.Date)
            .Distinct()
            .Count();
        return distinctCompletedDays >= AssimilationThreshold;
    }

    public bool? IsCompletedToday()
    {
        var today = DateTime.UtcNow.Date;
        var checkpoint = Checkpoints.FirstOrDefault(c => c.Date == today);
        return checkpoint?.Completed;
    }

    public int GetCompletedCountInWindow()
    {
        var windowStart = DateTime.UtcNow.Date.AddDays(-(AssimilationWindowDays - 1));
        return Checkpoints
            .Where(c => c.Completed && c.Date >= windowStart)
            .Select(c => c.Date)
            .Distinct()
            .Count();
    }
}