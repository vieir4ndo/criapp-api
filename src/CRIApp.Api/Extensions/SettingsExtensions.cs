using System.Diagnostics.CodeAnalysis;
using CRIApp.Domain.Settings;

namespace CRIApp.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class SettingsExtensions
{
    public static IServiceCollection AddSettingsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TwilioSettings>(configuration.GetSection(TwilioSettings.SectionName));
        services.Configure<MailjetSettings>(configuration.GetSection(MailjetSettings.SectionName));
        return services;
    }
}
