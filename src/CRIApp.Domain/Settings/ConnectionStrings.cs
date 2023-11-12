namespace CRIApp.Domain.Settings;

public class ConnectionStrings
{
    public static string SectionName { get; private set;  }= nameof(ConnectionStrings);
    public string Postgres { get; set; }
}