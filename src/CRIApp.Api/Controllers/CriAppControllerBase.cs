using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CRIApp.Api.Controllers;

public class CriAppControllerBase : ControllerBase
{
    private ILogger<CriAppControllerBase>? _logger;
    private ILogger<CriAppControllerBase> Logger =>
        _logger ??= HttpContext.RequestServices.GetRequiredService<ILogger<CriAppControllerBase>>();

    protected Guid GetUserId()
    {
        var userIdClaim = User.FindFirst("id")?.Value;

        return Guid.Parse(userIdClaim!);
    }

    protected void ValidateAndLog(BaseRequest request)
    {
        try
        {
            request.ValidateAndThrow();
        }
        catch (ValidationException ex)
        {
            Logger.LogWarning(
                "Validation failed on {Method} {Path}: {Errors}",
                HttpContext.Request.Method,
                HttpContext.Request.Path,
                string.Join("; ", ex.Errors));
            throw;
        }
    }
}