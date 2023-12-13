using CRIApp.Domain.Enums;

namespace CRIApp.Application.Dto;

public class UpdateChildDto
{
    public string? Name { get; set; }
    public DateTime? BirthDate { get; set; }
    public Sex? Sex { get; set; }
    public int? MonthsOfBreastFeeding { get; set; }
    public bool? IsPremature { get; set; }
}