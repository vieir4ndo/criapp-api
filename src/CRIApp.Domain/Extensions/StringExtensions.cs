using System.Globalization;

namespace CRIApp.Domain.Extensions;

public static class StringExtensions
{
    public static string ToTitleCase(this string value)
        => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.Trim().ToLower());
}
