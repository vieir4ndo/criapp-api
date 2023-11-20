using CRIApp.Domain.Enums;
using CRIApp.Domain.Helpers;

namespace CRIApp.Domain.Entities;

public class User : Entity
{
    public string Name { get; private set; }
    public DateTime BirthDate { get; private set; }
    public Sex Sex { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool Active { get; private set; }
    public int? NumberOfChildren { get; private set; }
    public FamilyType? FamilyType { get; private set; }
    public EducationalLevel? EducationalLevel { get; private set; }
    public bool IsAdmin { get; private set; } = false;
    public List<Answer> Answers { get; private set; } = new();
    public List<Child> Children { get; private set; } = new();
    public List<Question> Questions { get; private set; } = new();
    public List<AudiovisualResource> AudiovisualResources { get; private set; } = new();
    public List<Award> Awards { get; private set; } = new();
    public List<Comment> Comments { get; private set; } = new();
    public List<Forum> Forums { get; private set; } = new();
    public List<Faq> Faqs { get; private set; } = new();
    public void Activate() => Active = true;
    public void Deactivate() => Active = false;
    
    

    protected User() { }
    
    public User(
        string name,
        DateTime birthDate,
        Sex sex,
        string email,
        string password,
        FamilyType? familyType = null,
        EducationalLevel? educationalLevel = null,
        int? numberOfChildren = null
        )
    {
        Name = name;
        BirthDate = birthDate;
        Sex = sex;
        Email = email;
        FamilyType = familyType;
        EducationalLevel = educationalLevel;
        PasswordHash = PasswordHasher.HashPassword(password);
        NumberOfChildren = numberOfChildren;
        Activate();
        
        Name.IsNullOrEmpty("An user cannot have a null or empty name");
        BirthDate.IsDateTimeMinValue("An user has to have a valid birth date");
        Sex.IsInEnum("An user has to have a sex according to system values");
        Email.IsValidEmail("An user has to have a valid email");
        Active.IsActive("An user has to be active to be created");
        PasswordHash.IsNullOrEmpty("An user cannot have a null or empty password hash");
    }

    public bool VerifyPassword(string password)
    {
        return PasswordHasher.VerifyPassword(password, PasswordHash);
    }
}