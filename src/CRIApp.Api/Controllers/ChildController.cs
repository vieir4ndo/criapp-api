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
[Route("child")]
public class ChildController : CriAppControllerBase
{
    private readonly IChildService _service;
    private readonly IChildBookService _childBookService;

    public ChildController(IChildService service, IChildBookService childBookService, ILogger<ChildController> logger) : base(logger)
    {
        _service = service;
        _childBookService = childBookService;
    }
    
    [HttpPost]
    public async Task<ApiResponse<object>> Create(CreateChildRequest request)
    {
        ValidateAndLog(request);
        
        await _service.Create(request.ToDto(GetUserId()));

        return new ApiResponse<object>(true);
    }
    
    [HttpGet]
    public async Task<ApiResponse<List<ChildDto>>> GetAllById()
    {
        var data = await _service.GetAllById(GetUserId());

        return new ApiResponse<List<ChildDto>>(true, data);
    }
    
    [HttpPatch("{childId}")]
    public async Task<ApiResponse<object>> Update(UpdateChildRequest request, [FromRoute] Guid childId)
    {
        ValidateAndLog(request);
        
        await _service.Update(GetUserId(), childId, request.ToDto());

        return new ApiResponse<object>(true);
    }
    
    [HttpGet("{childId}")]
    public async Task<ApiResponse<ChildDto>> Get([FromRoute] Guid childId)
    {
        var child = await _service.GetAndValidateById(childId, GetUserId());

        return new ApiResponse<ChildDto>(true, new ChildDto(child));
    }

    [HttpGet("{childId}/book")]
    public async Task<ApiResponse<ChildBookDto>> GetBookScore([FromRoute] Guid childId)
    {
        var summary = await _childBookService.GetSummaryByChildId(childId, GetUserId());

        return new ApiResponse<ChildBookDto>(true, summary);
    }

    [HttpGet("{childId}/book/awards")]
    public async Task<ApiResponse<AwardDto>> GetBookAwards([FromRoute] Guid childId)
    {
        var awards = await _childBookService.GetAwardsByChildId(childId, GetUserId());

        return new ApiResponse<AwardDto>(true, awards);
    }

    [HttpGet("{childId}/book/recommendations")]
    public async Task<ApiResponse<RecommendationDto>> GetBookRecommendations([FromRoute] Guid childId)
    {
        var recommendations = await _childBookService.GetRecommendationsByChildId(childId, GetUserId());

        return new ApiResponse<RecommendationDto>(true, recommendations);
    }

    [HttpPost("{childId}/book/recommendations/{recommendationId}/checkpoint")]
    public async Task<ApiResponse<SaveCheckpointResultDto>> CreateCheckpoint(
        [FromRoute] Guid childId,
        [FromRoute] int recommendationId,
        [FromBody] CreateCheckpointRequest request)
    {
        ValidateAndLog(request);

        var result = await _childBookService.SaveCheckpoint(childId, GetUserId(), recommendationId, request.Completed);

        return new ApiResponse<SaveCheckpointResultDto>(true, result);
    }
}