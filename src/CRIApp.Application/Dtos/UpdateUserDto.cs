using CRIApp.Domain.Enums;

namespace CRIApp.Application.Dto;

public class UpdateUserDto
{
    public string? Name { get; set; }
    public DateTime? BirthDate { get; set; }
    public Sex? Sex { get; set; }
    public FamilyType? FamilyType { get; set; }
    public EducationalLevel? EducationalLevel { get; set; }
    public int? NumberOfChildren { get; set; }
}