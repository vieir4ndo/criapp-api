using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CRIApp.Api.Controllers;

public class CriAppControllerBase : ControllerBase
{
    private readonly ILogger _logger;

    public CriAppControllerBase(ILogger logger)
    {
        _logger = logger;
    }

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
            _logger.LogWarning(
                "Validation failed on {Method} {Path}: {Errors}",
                HttpContext.Request.Method,
                HttpContext.Request.Path,
                string.Join("; ", ex.Errors));
            throw;
        }
    }
}