using System.Globalization;

namespace CRIApp.Api.Controllers.Validators;

internal static class ValidationMessages
{
    private static string Lang => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    public static string PasswordStrength => Lang switch
    {
        "es" => "La contraseña debe tener al menos 8 caracteres y contener mayúscula, minúscula, dígito y carácter especial.",
        "pt" => "A senha deve ter pelo menos 8 caracteres e conter maiúscula, minúscula, dígito e caractere especial.",
        _ => "Password must be at least 8 characters and contain uppercase, lowercase, digit and special character."
    };

    public static string UsernameEmailOrPhone => Lang switch
    {
        "es" => "El nombre de usuario debe ser una dirección de correo electrónico o número de teléfono válido.",
        "pt" => "O nome de utilizador deve ser um endereço de email ou número de telefone válido.",
        _ => "Username must be a valid email address or phone number."
    };

    public static string UsernameEmail => Lang switch
    {
        "es" => "El nombre de usuario debe ser una dirección de correo electrónico válida.",
        "pt" => "O nome de utilizador deve ser um endereço de email válido.",
        _ => "Username must be a valid email address."
    };

    public static string BirthDateFormat => Lang switch
    {
        "es" => "La fecha de nacimiento debe estar en formato ISO 8601 (YYYY-MM-DD).",
        "pt" => "A data de nascimento deve estar no formato ISO 8601 (YYYY-MM-DD).",
        _ => "BirthDate must be in format ISO 8601 (YYYY-MM-DD)."
    };
}
