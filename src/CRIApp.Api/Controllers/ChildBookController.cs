using CRIApp.Api.Controllers.Requests;
using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CRIApp.Api.Controllers;

[ApiController]
[Authorize]
[Route("child-book")]
public class ChildBookController : CriAppControllerBase
{
    private readonly IChildBookService _childBookService;

    public ChildBookController(IChildBookService childBookService, ILogger<ChildBookController> logger) : base(logger)
    {
        _childBookService = childBookService;
    }

    [HttpGet("{childBookId}")]
    public async Task<ApiResponse<ChildBookDto>> GetById([FromRoute] Guid childBookId)
    {
        var summary = await _childBookService.GetSummaryByChildBookId(childBookId, GetUserId());
        return new ApiResponse<ChildBookDto>(true, summary);
    }

    [HttpGet("{childBookId}/awards")]
    public async Task<ApiResponse<AwardDto>> GetAwards([FromRoute] Guid childBookId)
    {
        var awards = await _childBookService.GetAwardsByChildBookId(childBookId, GetUserId());
        return new ApiResponse<AwardDto>(true, awards);
    }

    [HttpGet("{childBookId}/recommendations")]
    public async Task<ApiResponse<RecommendationDto>> GetRecommendations([FromRoute] Guid childBookId)
    {
        var recommendations = await _childBookService.GetRecommendationsByChildBookId(childBookId, GetUserId());
        return new ApiResponse<RecommendationDto>(true, recommendations);
    }

    [HttpPost("{childBookId}/recommendations/{childBookRecommendationId}/checkpoint")]
    public async Task<ApiResponse<SaveCheckpointResultDto>> CreateCheckpoint(
        [FromRoute] Guid childBookId,
        [FromRoute] Guid childBookRecommendationId,
        [FromBody] CreateCheckpointRequest request)
    {
        ValidateAndLog(request);
        var result = await _childBookService.SaveCheckpoint(childBookId, GetUserId(), childBookRecommendationId, request.Completed);
        return new ApiResponse<SaveCheckpointResultDto>(true, result);
    }
}
