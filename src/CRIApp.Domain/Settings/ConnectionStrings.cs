using System.Diagnostics.CodeAnalysis;

namespace CRIApp.Domain.Settings;

[ExcludeFromCodeCoverage]
public class ConnectionStrings
{
    public static string SectionName { get; private set;  }= nameof(ConnectionStrings);
    public required string Postgres { get; set; }
}