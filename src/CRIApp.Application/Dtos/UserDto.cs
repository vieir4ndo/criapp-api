using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;

namespace CRIApp.Application.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
    public Sex Sex { get; set; }
    public string Email { get; set; }
    public bool Active { get; set; }
    public int? NumberOfChildren { get; set; }
    public FamilyType? FamilyType { get; set; }
    public EducationalLevel? EducationalLevel { get; set; }
    public int Score { get; set; }
    public DateTime ScoreLastUpdatedAt { get; set; }
    public bool IsAdmin { get; set; }

    public UserDto(User user)
    {
        Id = user.Id;
        Name = user.Name;
        BirthDate = user.BirthDate;
        Sex = user.Sex;
        Email = user.Username;
        Active = user.Active;
        NumberOfChildren = user.NumberOfChildren;
        FamilyType = user.FamilyType;
        EducationalLevel = user.EducationalLevel;
        IsAdmin = user.IsAdmin;
        Score = user.Score;
        ScoreLastUpdatedAt = user.ScoreLastUpdatedAt;
    }
}