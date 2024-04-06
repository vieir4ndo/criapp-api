using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Application.Dtos;

public class CreateChildDto
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public Sex Sex { get; set; }
    public int MonthsOfBreastFeeding { get; set; }
    public Guid PrcId { get; set; }
    public bool IsPremature { get; set; }

    public Child ToDomainObject()
    {
        return new Child(Name, BirthDate, Sex, MonthsOfBreastFeeding, PrcId, IsPremature);
    }
}