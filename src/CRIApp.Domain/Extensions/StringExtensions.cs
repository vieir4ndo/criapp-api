using System.Globalization;

namespace CRIApp.Domain.Extensions;

public static class StringExtensions
{
    public static string ToTitleCase(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return value!;
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim().ToLower());
    }
}
