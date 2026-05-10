namespace CRIApp.Domain.Entities;

public class ChildBookAdvice
{
    public Guid ChildBooksId { get; set; }
    public int AdvicesId { get; set; }

    public ChildBook? ChildBook { get; set; }
    public Advice? Advice { get; set; }
}
