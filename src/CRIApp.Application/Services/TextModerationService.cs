using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CRIApp.Application.Abstractions;

namespace CRIApp.Application.Services;

public class TextModerationService : ITextModerationService
{
    private static readonly string[] BlockedWordSeed =
    [
        "arrombado", "arrombada", "asshole", "babaca", "bastard", "bitch", "bollocks", "cabron", "cabrona",
        "cacete", "caralho", "chingada", "chingar", "cojones", "coño", "cunt", "cuzao", "cuzona", "desgraca",
        "desgracado", "desgracada", "dick", "fdp", "foda", "fodase", "foder", "fodido", "fodida", "fuck",
        "fucker", "gilipollas", "hell", "hostia", "idiota", "imbecil", "joda", "joder", "merda", "mierda", "motherfucker",
        "otario", "otaria", "pendejo", "pendeja", "pieceofshit", "porra", "puta", "puto", "shit", "slut",
        "verga", "whore", "cu", "cuzinho", "viado", "viada", "viadinho", "viadinha", "marica", "maricon", "bicha", "faggot", "dyke"
    ];

    private static readonly string[] BlockedPhraseSeed =
    [
        "filho da puta", "go to hell", "hijo de puta", "piece of shit", "que se joda", "son of a bitch",
        "vai tomar no cu", "vaya a la mierda", "vete a la mierda"
    ];

    private static readonly HashSet<string> BlockedWords = new(BlockedWordSeed, StringComparer.Ordinal);
    private static readonly HashSet<string> BlockedPhrases = new(BlockedPhraseSeed, StringComparer.Ordinal);

    private static readonly Regex[] BlockedWordPatterns =
    [
        new("^viad(o|a|inho|inha|agem|ao|ona|os|as)$", RegexOptions.Compiled),
        new("^maric(a|on|ona|ones|onas)$", RegexOptions.Compiled),
        new("^cuz(ao|ona|inho|inha|oes)$", RegexOptions.Compiled)
    ];

    public bool ContainsBlockedContent(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var normalized = Normalize(text);
        var words = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (ContainsBlockedWord(words))
            return true;

        return ContainsBlockedPhrase(normalized);
    }

    private static bool ContainsBlockedWord(IEnumerable<string> words)
    {
        foreach (var word in words)
        {
            if (BlockedWords.Contains(word))
                return true;

            if (BlockedWordPatterns.Any(pattern => pattern.IsMatch(word)))
                return true;
        }

        return false;
    }

    private static bool ContainsBlockedPhrase(string normalized)
    {
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