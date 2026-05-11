using CRIApp.Api.Controllers.Responses;
using CRIApp.Domain.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRIApp.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("feature-flags")]
public class FeatureFlagsController : CriAppControllerBase
{
    private readonly FeatureSettings _settings;

    public FeatureFlagsController(IOptions<FeatureSettings> settings, ILogger<FeatureFlagsController> logger) : base(logger)
    {
        _settings = settings.Value;
    }

    [HttpGet]
    public ApiResponse<FeatureFlagsResponse> Get()
    {
        return new ApiResponse<FeatureFlagsResponse>(true, new FeatureFlagsResponse
        {
            PhoneNumberUsernameEnabled = _settings.PhoneNumberUsernameEnabled
        });
    }
}
