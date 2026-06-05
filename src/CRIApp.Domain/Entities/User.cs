using System.ComponentModel.DataAnnotations;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Extensions;

namespace CRIApp.Domain.Entities;

public class User : Entity
{
    public string Name { get; private set; } = null!;
    public DateTime BirthDate { get; private set; }
    public Sex Sex { get; private set; }
    public string Username { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public bool Active { get; private set; }
    public int? NumberOfChildren { get; private set; }
    public FamilyType? FamilyType { get; private set; }
    public EducationalLevel? EducationalLevel { get; private set; }
    public bool IsAdmin { get; private set; } = false;
    public List<Answer> Answers { get; private set; } = new();
    public List<Child> Children { get; private set; } = new();
    public List<Comment> Comments { get; private set; } = new();
    public List<Forum> Forums { get; private set; } = new();
    public List<UserResetPasswordCode> UserResetPasswordCodes { get; private set; } = new();
    public List<RefreshToken> RefreshTokens { get; private set; } = new();
    
    public void Activate() => Active = true;
    public void Deactivate() => Active = false;
    
    public virtual UsernameType UsernameType()
    {
        return new EmailAddressAttribute().IsValid(Username) ? Enums.UsernameType.Email : Enums.UsernameType.PhoneNumber;
    }
    protected User() { }
    
    public User(
        string name,
        DateTime birthDate,
        Sex sex,
        string username,
        string password,
        FamilyType? familyType = null,
        EducationalLevel? educationalLevel = null,
        int? numberOfChildren = null
        )
    {
        Name = name.ToTitleCase();
        BirthDate = birthDate;
        Sex = sex;
        Username = username;
        FamilyType = familyType;
        EducationalLevel = educationalLevel;
        PasswordHash = HashExtensions.HashPassword(password);
        NumberOfChildren = numberOfChildren;
        Activate();
        
        Name.IsNullOrEmpty("An user cannot have a null or empty name");
        BirthDate.IsDateTimeMinValue("An user has to have a valid birth date");
        Sex.IsInEnum("An user has to have a sex according to system values");
        Username.IsValidUsername("An user has to have a valid username");
        Active.IsActive("An user has to be active to be created");
        PasswordHash.IsNullOrEmpty("An user cannot have a null or empty password hash");
    }

    public bool VerifyPassword(string password)
    {
        return HashExtensions.VerifyPassword(password, PasswordHash);
    }
    
    public bool VerifyResetPasswordCode(string code)
    {
        var lastCode = UserResetPasswordCodes.OrderByDescending(x => x.CreatedAt).FirstOrDefault();

        if (lastCode is null)
        {
            return false;
        }
        
        return lastCode.ExpiresAt >= DateTime.Now.ToUniversalTime() && HashExtensions.VerifyPassword(code, lastCode.CodeHash);
    }

    public void Update(string? name, DateTime? birthDate, Sex? sex, EducationalLevel? educationalLevel, FamilyType? familyType, int? numberOfChildren)
    {
        Name = name?.ToTitleCase() ?? Name;
        BirthDate = birthDate ?? BirthDate;
        Sex = sex ?? Sex;
        FamilyType = familyType;
        EducationalLevel = educationalLevel;
        NumberOfChildren = numberOfChildren ?? NumberOfChildren;
    }
    
    public void UpdatePassword(string password)
    {
        PasswordHash = HashExtensions.HashPassword(password);
        
        PasswordHash.IsNullOrEmpty("An user cannot have a null or empty password hash");
    }

    public void SetAsAdmin()
    {
        IsAdmin = true;
    }
}