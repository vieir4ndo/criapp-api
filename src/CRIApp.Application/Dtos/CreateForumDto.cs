using CRIApp.Domain.Entities;

namespace CRIApp.Application.Dtos;

public class CreateForumDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public Guid PrcId { get; set; }

    public Forum ToDomainObject()
    {
        return new Forum(Name, Description, PrcId);
    }
}