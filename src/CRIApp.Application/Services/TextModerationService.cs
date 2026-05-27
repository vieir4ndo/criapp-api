using System.Globalization;
using System.Text;
using CRIApp.Application.Abstractions;

namespace CRIApp.Application.Services;

public class TextModerationService : ITextModerationService
{
    private static readonly HashSet<string> BlockedWords =
    [
        "arrombado", "arrombada", "asshole", "babaca", "bastard", "bitch", "bollocks", "cabron", "cabrona",
        "cacete", "caralho", "chingada", "chingar", "cojones", "coño", "cunt", "cuzao", "cuzona", "desgraca",
        "desgracado", "desgracada", "dick", "fdp", "foda", "fodase", "foder", "fodido", "fodida", "fuck",
        "fucker", "gilipollas", "hostia", "idiota", "imbecil", "joder", "merda", "mierda", "motherfucker",
        "otario", "otaria", "pendejo", "pendeja", "pieceofshit", "porra", "puta", "puto", "shit", "slut",
        "verga", "whore"
    ];

    private static readonly HashSet<string> BlockedPhrases =
    [
        "filho da puta", "go to hell", "hijo de puta", "piece of shit", "que se joda", "son of a bitch",
        "vai tomar no cu", "vaya a la mierda", "vete a la mierda"
    ];

    public bool ContainsBlockedContent(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var normalized = Normalize(text);
        var words = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (words.Any(word => BlockedWords.Contains(word)))
            return true;

        return BlockedPhrases.Any(phrase => normalized.Contains(phrase, StringComparison.Ordinal));
    }

    private static string Normalize(string text)
    {
        text = text
            .Replace('0', 'o')
            .Replace('1', 'i')
            .Replace('3', 'e')
            .Replace('4', 'a')
            .Replace('5', 's')
            .Replace('7', 't');

        var decomposed = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(decomposed.Length);

        foreach (var character in decomposed)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
            if (unicodeCategory == UnicodeCategory.NonSpacingMark)
                continue;

            sb.Append(char.IsLetterOrDigit(character) ? char.ToLowerInvariant(character) : ' ');
        }

        var normalized = sb.ToString().Normalize(NormalizationForm.FormC);

        return string.Join(' ', normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
    }
}