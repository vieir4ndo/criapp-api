using System.Globalization;
using CRIApp.Api.Controllers.Validators;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Enums;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class CreateUserRequest : BaseRequest
{
    public string Name { get; set; }
    public string BirthDate { get; set; }
    public Sex Sex { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public FamilyType? FamilyType { get; set; }
    public EducationalLevel? EducationalLevel { get; set; }
    public int? NumberOfChildren { get; set; }

    public CreateUserDto ToDto()
    {
        return new CreateUserDto()
        {
            Name = Name,
            BirthDate = DateTime.ParseExact(BirthDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None).ToUniversalTime(),
            Sex = Sex,
            Username = Username,
            Password = Password,
            FamilyType = FamilyType,
            EducationalLevel = EducationalLevel,
            NumberOfChildren = NumberOfChildren
        };
    }

    protected override ValidationResult Validate()
    {
        return new CreateUserRequestValidator().Validate(this);
    }
}