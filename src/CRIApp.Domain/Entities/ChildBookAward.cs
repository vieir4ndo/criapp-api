namespace CRIApp.Domain.Entities;

public class ChildBookAward
{
    public Guid ChildBooksId { get; set; }
    public Guid AwardsId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ChildBook? ChildBook { get; set; }
    public Award? Award { get; set; }
}
