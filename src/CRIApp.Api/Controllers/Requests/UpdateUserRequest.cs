using System.Globalization;
using CRIApp.Api.Controllers.Validators;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Enums;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class UpdateUserRequest : BaseRequest
{
    public string? Name { get; set; }
    public string? BirthDate { get; set; }
    public Sex? Sex { get; set; }
    public FamilyType? FamilyType { get; set; }
    public EducationalLevel? EducationalLevel { get; set; }
    public int? NumberOfChildren { get; set; }
    
    protected override ValidationResult Validate()
    {
        return new UpdateUserRequestValidator().Validate(this);
    }
    
    public UpdateUserDto ToDto()
    {
        return new UpdateUserDto()
        {
            Name = Name,
            BirthDate = BirthDate is not null ? DateTime.ParseExact(BirthDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None).ToUniversalTime() : null,
            Sex = Sex,
            FamilyType = FamilyType,
            EducationalLevel = EducationalLevel,
            NumberOfChildren = NumberOfChildren
        };
    }
}