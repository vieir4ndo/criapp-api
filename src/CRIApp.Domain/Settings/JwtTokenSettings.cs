using System.Diagnostics.CodeAnalysis;

namespace CRIApp.Domain.Settings;

[ExcludeFromCodeCoverage]
public class JwtTokenSettings
{
    public static string SectionName { get; private set;  }= nameof(JwtTokenSettings);
    public required string Key { get; set; }
    public required string Issuer { get; set; }
    public required int ExpiresInMinutes { get; set; }
    public required int RefreshTokenExpiresInDays { get; set; }
}