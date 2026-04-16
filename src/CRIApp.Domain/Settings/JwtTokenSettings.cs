using System.Diagnostics.CodeAnalysis;

namespace CRIApp.Domain.Settings;

[ExcludeFromCodeCoverage]
public class JwtTokenSettings
{
    public static string SectionName { get; private set;  }= nameof(JwtTokenSettings);
    public string Key { get; set; }
    public string Issuer { get; set; }
    public int ExpiresInMinutes { get; set; } 
}