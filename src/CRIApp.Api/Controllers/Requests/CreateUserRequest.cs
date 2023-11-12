using CRIApp.Api.Controllers.Validators;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Enums;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class CreateUserRequest : BaseRequest
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public Sex Sex { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public FamilyType? FamilyType { get; set; }
    public EducationalLevel? EducationalLevel { get; set; }

    public CreateUserDto ToDto()
    {
        return new CreateUserDto()
        {
            Name = Name,
            BirthDate = BirthDate,
            Sex = Sex,
            Email = Email,
            Password = Password,
            FamilyType = FamilyType,
            EducationalLevel = EducationalLevel
        };
    }

    protected override ValidationResult Validate()
    {
        return new CreateUserRequestValidator().Validate(this);
    }
}