using CRIApp.Domain.Entities;

namespace CRIApp.Application.Dtos;

public class ForumDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string PrcName { get; set; }
    public string PrcId { get; set; }
    public List<CommentDto> Comments { get; set; }

    public ForumDto(Forum forum)
    {
        Id = forum.Id;
        Name = forum.Name;
        Description = forum.Description;
        PrcName = forum.Prc.Name;
        Comments = forum.Comments.Select(x => new CommentDto(x)).ToList();
        PrcId = forum.Prc.Id.ToString();
    }
}