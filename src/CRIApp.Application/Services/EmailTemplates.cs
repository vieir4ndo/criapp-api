namespace CRIApp.Application.Services;

internal static class EmailTemplates
{
    private const string LogoBase64 = "PHN2ZwogICAgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIgogICAgd2lkdGg9IjExNS4yOSIKICAgIGhlaWdodD0iMTE1LjI5IgogICAgdmlld0JveD0iMCAwIDExNS4yOSAxMTUuMjkiPg0KCTxwYXRoIGQ9Ik0zMC43NCwzNS4zNG0tNy4xNywwYTcuMTcsNy4xNyAwLDEgMSwxNC4zNCAwYTcuMTcsNy4xNyAwLDEgMSwtMTQuMzQgMCIgc3Ryb2tlLXdpZHRoPSIwIiBmaWxsPSIjZmZmIi8+DQoJPHBhdGggZD0iTTUxLjEyLDUzLjI0bS00LjksMGE0LjksNC45IDAsMSAxLDkuOCAwYTQuOSw0LjkgMCwxIDEsLTkuOCAwIiBzdHJva2Utd2lkdGg9IjAiIGZpbGw9IiNmZmYiLz4NCgk8cGF0aCBkPSJNNjkuODgsMzUuMzRtLTcuMTcsMGE3LjE3LDcuMTcgMCwxIDEsMTQuMzQgMGE3LjE3LDcuMTcgMCwxIDEsLTE0LjM0IDAiIHN0cm9rZS13aWR0aD0iMCIgZmlsbD0iI2ZmZiIvPg0KCTxwYXRoIGQ9Im0zMy4wNSw0NS4wM2MwLjYyLDQuMTkgMy43LDIwLjA1IDEzLjg0LDE1LjE5IDEuMjUsLTAuNiAyLjYsLTAuOTggMy45OSwtMS4wMiAwLjA4LDAgMC4xNywwIDAuMjYsLTAuMDIgMS4zOCwtMC4xMiAyLjc3LDAuMDUgNC4wNywwLjUzIDMuMDksMS4xMiA4Ljk1LDEuMDkgMTIuMzMsLTEyLjYxIDAuMjcsLTEuMDkgMS4yNSwtMS44MiAyLjM4LC0xLjgzIDAuOSwwIDIuMDIsLTAuMjIgMy4xMywtMS4wMiAwLjkzLC0wLjY3IDIuMjUsLTAuMjkgMi42NywwLjc4IDMuODIsOS42MiAxNy4wNyw0My42NyAxMy44Niw0NC42IC0zLjc0LDEuMDggLTEzLjg4LC0zNy44IC0xNS40NiwtMzcuNDFzLTYuNjksMzAuMTcgLTMuNzQsMzcuNDMgLTEwLjE0LC02LjUyIC02LjEsLTIyLjY2YzQuMDQsLTE2LjE0IC03LjE5LC0yLjA3IC03LjQ4LC0wLjM5bC0wLjY4LDIxLjc1Yy0wLjA2LDEuODQgLTEuNTYsMy4zIC0zLjQsMy4zaDBjLTEuOTMsMCAtMy40OCwtMS42IC0zLjQsLTMuNTMgMC4yOCwtNy4wNCAxLjIyLC0yMSAtNC40MywtMjMuMzlzLTIzLjY1LC0zLjQ1IC0yNi4xNSw2LjFjLTIuNSw5LjU1IDE4LjA4LDE4LjcgMTkuMTYsMTkuOThzLTQ3LjIzLC0zLjg2IC0xNC42LC00Ny42NmMwLjUyLC0wLjcgMS41NSwtMC43NyAyLjE0LC0wLjEzIDEuMjYsMS4zNSAzLjYsMy4wNCA2LjYyLDEuNTIgMC40MSwtMC4yMSAwLjkxLDAuMDMgMC45OCwwLjQ5WiIgc3Ryb2tlLXdpZHRoPSIwIiBmaWxsPSIjZmZlMjZhIi8+DQoJPHBhdGggZD0ibTQ1LjU2LDc3LjYzYzAuMTUsLTIuNCAwLjc5LC00LjMzIDEuMjksLTUuNTYgMCwwIDIuNSwtOC4wMiAtMTEuNSwtNy4yNmwzLjc3LDI1LjY1cy0xLjczLC0xNi4zNCAxLjIyLC0xNS4yMWMyLjk1LDEuMTQgMy42MywxNS4yMSA2LjEzLDE1LjIxIDAuMTYsMCAwLjI4LC0wLjA0IDAuMzksLTAuMSAxLjQ2LC0wLjk2IC0xLjcsLTYuMzkgLTEuMjksLTEyLjcyWk0zOC45LDcxLjQ2Yy0xLjk3LC0xLjE0IC0wLjY4LC00LjM1IC0wLjY4LC00LjM1IDEyLjcxLDIuMDggMi42NSw1LjQ5IDAuNjgsNC4zNVoiIHN0cm9rZS13aWR0aD0iMCIgZmlsbD0iI2ZmZTI2YSIvPg0KCTxwYXRoIGQ9Im05Ny45NCw2OS4wMWMtMS42MiwtMy41NyAtNy4yMiwtMS44MSAtMTAuMDcsLTAuNTEgMCwwIDIuOTIsMTguNTQgNC4wNiwxOS4zNSAxLjE0LDAuODEgLTAuODksLTEyLjk5IDAsLTE0LjEzIDAuODksLTEuMTQgNy42MywtMS4xNCA2LjAxLC00LjcxWk05MS40NSw2OS4zOXMyLjAyLC0yLjQ3IDQuMjIsLTAuODljMi4yLDEuNTcgLTIuMjcsMi44NyAtMywzLjE4IC0wLjczLDAuMzIgLTEuMjIsLTIuMjkgLTEuMjIsLTIuMjlaIiBzdHJva2Utd2lkdGg9IjAiIGZpbGw9IiNmZmUyNmEiLz4NCgk8cGF0aCBkPSJtMTA5LjE1LDcwLjQ3Yy0wLjg3LC0zLjgzIC02LjcxLC0zLjIyIC05Ljc2LC0yLjUyIDAsMCAtMC44NiwxOC43NSAwLjA5LDE5Ljc3IDAuOTUsMS4wMiAxLjczLC0xMi45IDIuODMsLTEzLjg0IDEuMSwtMC45MyA3LjcsMC40MiA2LjgzLC0zLjQxWk0xMDIuNzEsNjkuNTRzMi40NywtMi4wMSA0LjMyLC0wLjAzYzEuODQsMS45OSAtMi44LDIuMzUgLTMuNTgsMi41MiAtMC43OCwwLjE2IC0wLjczLC0yLjQ5IC0wLjczLC0yLjQ5WiIgc3Ryb2tlLXdpZHRoPSIwIiBmaWxsPSIjZmZlMjZhIi8+DQo8L3N2Zz4NCg==";

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
                                    <img src="data:image/svg+xml;base64,{LogoBase64}"
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
