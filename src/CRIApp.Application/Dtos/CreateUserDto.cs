using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Application.Dtos;

public class CreateUserDto
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public Sex Sex { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public FamilyType? FamilyType { get; set; }
    public EducationalLevel? EducationalLevel { get; set; }
    public int? NumberOfChildren { get; set; }

    public User ToDomainObject()
    {
        return new User(Name, BirthDate, Sex, Email, Password, FamilyType, EducationalLevel, NumberOfChildren);
    }
}