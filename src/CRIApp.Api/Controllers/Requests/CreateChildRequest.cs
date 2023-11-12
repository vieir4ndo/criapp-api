using CRIApp.Api.Controllers.Validators;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Enums;
using FluentValidation.Results;

namespace CRIApp.Api.Controllers.Requests;

public class CreateChildRequest : BaseRequest
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public Sex Sex { get; set; }
    public int MonthsOfBreastFeeding { get; set; }
    public Guid PrcId { get; set; }

    public CreateChildDto ToDto()
    {
        return new CreateChildDto()
        {
            Name = Name,
            BirthDate = BirthDate,
            Sex = Sex,
            MonthsOfBreastFeeding = MonthsOfBreastFeeding,
            PrcId = PrcId
        };
    }
    
    protected override ValidationResult Validate()
    {
        return new CreateChildRequestValidator().Validate(this);
    }
}