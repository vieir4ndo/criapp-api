using CRIApp.Domain.Enums;

namespace CRIApp.Domain.Entities;

public class Child : Entity
{
    public string Name { get; private set; }
    public DateTime BirthDate { get; private set; }
    public Sex Sex { get; private set; }
    public int MonthsOfBreastFeeding { get; private set; }
    public Guid PrcId { get; private set; }
    public User Prc { get; private set; }
    public List<Answer> Answers { get; private set; }

    public Child(string name, DateTime birthDate, Sex sex, int monthsOfBreastFeeding, Guid prcId)
    {
        Name = name;
        BirthDate = birthDate;
        Sex = sex;
        MonthsOfBreastFeeding = monthsOfBreastFeeding;
        PrcId = prcId;
        
        Name.IsNullOrEmpty("A child cannot have a null or empty name");
        BirthDate.IsDateTimeMinValue("A child has to have a valid birth date");
        Sex.IsInEnum("A child has to have a sex according to system values");
        MonthsOfBreastFeeding.IsPositive("A child has to have a valid value for months of breast feeding");
        PrcId.IsEmptyGuid("A child has to have a PRC associated");
    }
}