using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Application.Dtos;

public class AwardDto
{
    public Guid Id { get; set; }
    public AwardType Type { get; set; }

    public AwardDto(Award award)
    {
        Id = award.Id;
        Type = award.Type;
    }
}
