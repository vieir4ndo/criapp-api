using Microsoft.AspNetCore.Mvc;

namespace CRIApp.Api.Controllers;

public class CriAppControllerBase : ControllerBase
{
    protected Guid GetUserId()
    {
        var userIdClaim = User.FindFirst("id")?.Value;

        return Guid.Parse(userIdClaim!);
    }
}