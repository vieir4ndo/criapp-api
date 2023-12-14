using System.Text;

namespace CRIApp.Domain.Extensions;

public class RandomExtensions
{
    public static string GenerateRandomString(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringBuilder = new StringBuilder(length);

        for (var i = 0; i < length; i++)
        {
            var index = random.Next(chars.Length);
            stringBuilder.Append(chars[index]);
        }

        return stringBuilder.ToString();
    }
}