using System.Diagnostics.CodeAnalysis;

namespace CRIApp.Domain.Settings;

[ExcludeFromCodeCoverage]
public class FeatureSettings
{
    public static string SectionName { get; } = nameof(FeatureSettings);
    public bool PhoneNumberUsernameEnabled { get; set; } = false;

    public static FeatureSettings Current { get; private set; } = new();

    public static void Initialize(FeatureSettings settings)
    {
        Current = settings;
    }
}
