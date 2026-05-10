using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using System.Globalization;

namespace CRIApp.Application.Dtos;

public class AwardDto
{
    public Guid Id { get; set; }
    public AwardType Type { get; set; }
    public string Text { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }

    public AwardDto(Award award)
    {
        var lang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        Text = lang switch
        {
            "en" => award.TextEn ?? award.Text,
            "pt" => award.TextPt ?? award.Text,
            _ => award.Text
        };
        Description = lang switch
        {
            "en" => award.DescriptionEn ?? award.Description,
            "pt" => award.DescriptionPt ?? award.Description,
            _ => award.Description
        };
        Id = award.Id;
        Type = award.Type;
        Image = award.Image != null ? Convert.ToBase64String(award.Image) : null;
    }
}
