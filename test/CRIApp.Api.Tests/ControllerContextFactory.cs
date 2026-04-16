using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRIApp.Api.Tests;

internal static class ControllerContextFactory
{
    public static ControllerContext CreateAuthenticated(Guid userId)
    {
        var claims = new List<Claim> { new Claim("id", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        return new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }
}
