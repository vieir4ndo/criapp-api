using System.Security.Cryptography;
using System.Text;

namespace CRIApp.Domain.Helpers;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

        var builder = new StringBuilder();
        foreach (var t in hashedBytes)
        {
            builder.Append(t.ToString("x2"));
        }

        return builder.ToString();
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var hashedInput = HashPassword(password);
        return string.Equals(hashedInput, hashedPassword);
    }
}