using System.Globalization;
using CRIApp.Api.Controllers.Validators;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Enums;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class CreateChildRequest : BaseRequest
{
    public required string Name { get; set; }
    public required string BirthDate { get; set; }
    public required Sex Sex { get; set; }
    public required int MonthsOfBreastFeeding { get; set; }
    public required bool IsPremature { get; set; }

    public CreateChildDto ToDto(Guid prcId)
    {
        return new CreateChildDto()
        {
            Name = Name,
            BirthDate = DateTime.ParseExact(BirthDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None).ToUniversalTime(),
            Sex = Sex,
            MonthsOfBreastFeeding = MonthsOfBreastFeeding,
            PrcId = prcId,
            IsPremature = IsPremature
        };
    }
    
    protected override ValidationResult Validate()
    {
        return new CreateChildRequestValidator().Validate(this);
    }
}