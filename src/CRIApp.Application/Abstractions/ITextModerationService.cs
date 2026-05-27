namespace CRIApp.Application.Abstractions;

public interface ITextModerationService
{
    bool ContainsBlockedContent(string text);
}