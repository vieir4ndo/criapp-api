using System.ComponentModel.DataAnnotations;
using CRIApp.Domain.Exceptions;

namespace CRIApp.Domain;

public static class Validations
{
    public static void IsNullOrEmpty(this string text, string message)
    {
        if (string.IsNullOrEmpty(text))
            throw new DomainException(message);
    }
    
    
    public static void HasElements<T>(this IList<T> list, string message)
    {
        if (list.Any())
            throw new DomainException(message);
    }

    public static void IsEmptyGuid(this Guid guid, string message)
    {
        if (guid == Guid.Empty)
            throw new DomainException(message);
    }

    public static void IsActive(this bool active, string message)
    {
        if (active is false)
            throw new DomainException(message);
    }

    public static void IsValueInRange(this int value, int min, int max, string message)
    {
        if (value < min || value > max)
            throw new DomainException(message);
    }
    
    public static void IsDateTimeMinValue(this DateTime dateTime, string message)
    {
        if (dateTime == DateTime.MinValue)
            throw new DomainException(message);
    }
    
    public static void IsDateOnlyMinValue(this DateOnly dateOnly, string message)
    {
        if (dateOnly == DateOnly.MinValue)
            throw new DomainException(message);
    }
    
    public static void IsInEnum<T>(this T value, string message) where T : struct, Enum
    {
        if (!Enum.IsDefined(value))
            throw new DomainException(message);
    }
    
    public static void IsValidUsername(this string username, string message)
    {
        if (!new EmailAddressAttribute().IsValid(username) && !new PhoneAttribute().IsValid(username))
            throw new DomainException(message);
    }

    public static void IsPositive(this int number, string message)
    {
        if (number < 0)
            throw new DomainException(message);
    }
}