using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace CRIApp.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class FluentValidationExtensions
{
    public static IServiceCollection AddFluentValidationLocalization(this IServiceCollection services)
    {
        ValidatorOptions.Global.LanguageManager.Enabled = true;
        ValidatorOptions.Global.LanguageManager.Culture = new System.Globalization.CultureInfo("es-ES");
        return services;
    }
}
