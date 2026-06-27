using CRIApp.Domain.Entities;

namespace CRIApp.Application.Dtos;

public class CreateCommentDto
{
    public required string Text { get; set; }
    public Guid PrcId { get; set; }
    public Guid ForumId { get; set; }

    public Comment ToDomainObject()
    {
        return new Comment(Text, ForumId, PrcId);
    }
}