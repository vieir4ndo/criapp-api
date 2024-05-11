namespace CRIApp.Application.Dtos;

public class CreateCommentDto
{
    public string Text { get; set; }
    public Guid PrcId { get; set; }
    public Guid ForumId { get; set; }
}