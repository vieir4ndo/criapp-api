using System.Globalization;
using CRIApp.Api.Controllers.Validators;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Enums;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class UpdateChildRequest : BaseRequest
{
    public string? Name { get; set; }
    public string? BirthDate { get; set; }
    public Sex? Sex { get; set; }
    public int? MonthsOfBreastFeeding { get; set; }
    public bool? IsPremature { get; set; }
    protected override ValidationResult Validate()
    {
        return new UpdateChildRequestValidator().Validate(this);
    }
    
    public UpdateChildDto ToDto()
    {
        return new UpdateChildDto()
        {
            Name = Name,
            BirthDate = BirthDate is not null ? DateTime.ParseExact(BirthDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None).ToUniversalTime() : null,
            Sex = Sex,
            MonthsOfBreastFeeding = MonthsOfBreastFeeding,
            IsPremature = IsPremature
        };
    }
}