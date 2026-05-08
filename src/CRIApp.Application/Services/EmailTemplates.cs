namespace CRIApp.Application.Services;

internal static class EmailTemplates
{
    private const string LogoBase64 = "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";

    public static string AccountCreation(string recipientEmail) =>
        BuildHtml(
            title: "¡Bienvenido/a a CRIApp!",
            preheader: "Tu cuenta ha sido creada con éxito.",
            body: $"""
                <h1 style="margin:0 0 16px;font-size:26px;font-weight:700;color:#1a1a2e;">
                    ¡Bienvenido/a a CRIApp!
                </h1>
                <p style="margin:0 0 16px;font-size:16px;color:#555;line-height:1.6;">
                    Tu cuenta ha sido creada exitosamente. Ahora puedes acceder a la app y comenzar
                    a gestionar la información de intervención educativa de tu familia.
                </p>
                <p style="margin:0 0 24px;font-size:16px;color:#555;line-height:1.6;">
                    Cuenta registrada con: <strong style="color:#2C64C6;">{System.Web.HttpUtility.HtmlEncode(recipientEmail)}</strong>
                </p>
                <a href="#"
                   style="display:inline-block;padding:14px 32px;background:linear-gradient(135deg,#2C64C6,#A966CC);
                          color:#fff;text-decoration:none;border-radius:8px;font-size:16px;font-weight:600;
                          letter-spacing:0.5px;">
                    Abrir CRIApp
                </a>
            """);

    public static string ForgotPassword(string code) =>
        BuildHtml(
            title: "Recuperación de contraseña",
            preheader: "Tu código de verificación está aquí.",
            body: $"""
                <h1 style="margin:0 0 16px;font-size:26px;font-weight:700;color:#1a1a2e;">
                    Recupera tu contraseña
                </h1>
                <p style="margin:0 0 24px;font-size:16px;color:#555;line-height:1.6;">
                    Recibimos una solicitud para restablecer la contraseña de tu cuenta.
                    Usa el siguiente código de verificación:
                </p>
                <div style="margin:0 auto 32px;display:inline-block;padding:20px 40px;
                            background:linear-gradient(135deg,#2C64C6,#A966CC);
                            border-radius:12px;text-align:center;">
                    <span style="font-size:36px;font-weight:800;letter-spacing:10px;color:#FFE26A;
                                 font-family:'Courier New',monospace;">
                        {System.Web.HttpUtility.HtmlEncode(code)}
                    </span>
                </div>
                <p style="margin:0 0 8px;font-size:14px;color:#888;line-height:1.6;">
                    Este código expira en <strong>10 minutos</strong>. Si no solicitaste este cambio, ignora este mensaje.
                </p>
            """);

    private static string BuildHtml(string title, string preheader, string body) => $"""
        <!DOCTYPE html>
        <html lang="es">
        <head>
            <meta charset="UTF-8" />
            <meta name="viewport" content="width=device-width, initial-scale=1.0" />
            <meta http-equiv="X-UA-Compatible" content="IE=edge" />
            <title>{System.Web.HttpUtility.HtmlEncode(title)}</title>
        </head>
        <body style="margin:0;padding:0;background-color:#f0f4f8;font-family:'Segoe UI',Arial,sans-serif;">
            <span style="display:none;max-height:0;overflow:hidden;">
                {System.Web.HttpUtility.HtmlEncode(preheader)}&nbsp;
            </span>

            <table role="presentation" cellpadding="0" cellspacing="0" width="100%"
                   style="background-color:#f0f4f8;padding:40px 16px;">
                <tr>
                    <td align="center">
                        <table role="presentation" cellpadding="0" cellspacing="0" width="600"
                               style="max-width:600px;width:100%;background:#ffffff;border-radius:16px;
                                      overflow:hidden;box-shadow:0 4px 24px rgba(44,100,198,0.12);">

                            <!-- Header -->
                            <tr>
                                <td style="background:linear-gradient(135deg,#2C64C6 0%,#A966CC 100%);
                                           padding:36px 40px;text-align:center;">
                                    <img src="data:image/png;base64,{LogoBase64}"
                                         alt="CRIApp" width="80" height="80"
                                         style="display:block;margin:0 auto 16px;border-radius:16px;" />
                                    <span style="font-size:28px;font-weight:800;color:#FFE26A;
                                                 letter-spacing:2px;font-family:'Segoe UI',Arial,sans-serif;">
                                        CRIApp
                                    </span>
                                </td>
                            </tr>

                            <!-- Body -->
                            <tr>
                                <td style="padding:40px 48px;text-align:left;">
                                    {body}
                                </td>
                            </tr>

                            <!-- Divider -->
                            <tr>
                                <td style="padding:0 48px;">
                                    <hr style="border:none;border-top:1px solid #e8edf5;margin:0;" />
                                </td>
                            </tr>

                            <!-- Footer -->
                            <tr>
                                <td style="padding:28px 48px;text-align:center;">
                                    <p style="margin:0 0 8px;font-size:13px;color:#aaa;">
                                        Este es un mensaje automático de <strong style="color:#2C64C6;">CRIApp</strong>.
                                        Por favor, no respondas a este correo.
                                    </p>
                                    <p style="margin:0;font-size:12px;color:#ccc;">
                                        © {DateTime.UtcNow.Year} CRIApp — Todos los derechos reservados.
                                    </p>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </body>
        </html>
        """;
}
