using CRIApp.Domain.Entities;

namespace CRIApp.Application.Dtos;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public string PrcName { get; set; }
    public string PrcId { get; set; }

    public CommentDto(Comment comment)
    {
        Id = comment.Id;
        Text = comment.Text;
        PrcName = comment.Prc.Name;
        PrcId = comment.Prc.Id.ToString();
    }
}