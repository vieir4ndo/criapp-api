using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Application.Dtos;

public class ChildDto
{
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public Sex Sex { get; set; }
    public int MonthsOfBreastFeeding { get; set; }
    public Guid PrcId { get; set; }
    public bool IsPremature { get; set; }

    public ChildDto(Child child)
    {
        Name = child.Name;
        BirthDate = child.BirthDate;
        Sex = child.Sex;
        MonthsOfBreastFeeding = child.MonthsOfBreastFeeding;
        PrcId = child.PrcId;
        IsPremature = child.IsPremature;
    }
}