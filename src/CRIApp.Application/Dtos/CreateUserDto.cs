using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Application.Dtos;

public class CreateUserDto
{
    public required string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public Sex Sex { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public FamilyType? FamilyType { get; set; }
    public EducationalLevel? EducationalLevel { get; set; }
    public int? NumberOfChildren { get; set; }

    public User ToDomainObject()
    {
        return new User(Name, BirthDate, Sex, Username, Password, FamilyType, EducationalLevel, NumberOfChildren);
    }
}