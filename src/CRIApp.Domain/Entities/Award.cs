using CRIApp.Domain.Enums;

namespace CRIApp.Domain.Entities;

public class Award : Entity
{
    public AwardType Type { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public string? TextEn { get; private set; }
    public string? TextPt { get; private set; }
    public string? Description { get; private set; }
    public string? DescriptionEn { get; private set; }
    public string? DescriptionPt { get; private set; }
    public List<ChildBook> ChildBooks { get; private set; } = new();

    public Award(AwardType type, string text)
    {
        Type = type;
        Text = text;
    }

    public Award WithTranslations(string? textEn, string? textPt, string? description = null, string? descriptionEn = null, string? descriptionPt = null)
    {
        TextEn = textEn;
        TextPt = textPt;
        Description = description;
        DescriptionEn = descriptionEn;
        DescriptionPt = descriptionPt;
        return this;
    }


}