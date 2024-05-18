using CRIApp.Domain.Entities;

namespace CRIApp.Application.Dtos;

public class CreateForumDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid PrcId { get; set; }

    public Forum ToDomainObject()
    {
        return new Forum(Name, Description, PrcId);
    }
}