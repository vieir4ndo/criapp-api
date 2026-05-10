namespace CRIApp.Domain.Entities;

public class UserAdvice
{
    public Guid UsersId { get; set; }
    public int AdvicesId { get; set; }

    public User? User { get; set; }
    public Advice? Advice { get; set; }
}

