using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Application.Resources;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Services;

public class ChildBookService : IChildBookService
{
    private readonly IChildBookRepository _childBookRepository;
    private readonly IRecommendationCheckpointRepository _checkpointRepository;
    private readonly IChildService _childService;
    private readonly ILogger<ChildBookService> _logger;
    private readonly IStringLocalizer<AppMessages> _localizer;

    public ChildBookService(
        IChildBookRepository childBookRepository,
        IRecommendationCheckpointRepository checkpointRepository,
        IChildService childService,
        ILogger<ChildBookService> logger,
        IStringLocalizer<AppMessages> localizer)
    {
        _childBookRepository = childBookRepository;
        _checkpointRepository = checkpointRepository;
        _childService = childService;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<ChildBookDto> GetSummaryByChildId(Guid childId, Guid prcId)
    {
        _ = await _childService.GetAndValidateById(childId, prcId);

        var childBook = await _childBookRepository.GetByChildId(childId);

        if (childBook is null)
        {
            _logger.LogWarning("ChildBook for child {ChildId} not found", childId);
            throw new NotFoundException(_localizer["ChildBookNotFound"]);
        }

        return new ChildBookDto(childBook);
    }

    public async Task<List<AwardDto>> GetAwardsByChildId(Guid childId, Guid prcId)
    {
        _ = await _childService.GetAndValidateById(childId, prcId);

        var childBook = await _childBookRepository.GetByChildId(childId);

        if (childBook is null)
        {
            _logger.LogWarning("ChildBook for child {ChildId} not found", childId);
            throw new NotFoundException(_localizer["ChildBookNotFound"]);
        }

        return (await _childBookRepository.GetAwardsByChildBookId(childBook.Id))
            .Select(a => new AwardDto(a.Aw, a.AssignedAt)).ToList();
    }

    public async Task<List<RecommendationDto>> GetRecommendationsByChildId(Guid childId, Guid prcId)
    {
        _ = await _childService.GetAndValidateById(childId, prcId);

        var childBook = await _childBookRepository.GetByChildIdWithRecommendations(childId);

        if (childBook is null)
        {
            _logger.LogWarning("ChildBook for child {ChildId} not found", childId);
            throw new NotFoundException(_localizer["ChildBookNotFound"]);
        }

        var recommendations = await _childBookRepository.GetRecommendationsByChildBookId(childBook.Id);

        return recommendations.Select(tuple =>
            new RecommendationDto(tuple.Rec, tuple.AssignedAt)
        ).ToList();
    }
    
    public async Task<SaveCheckpointResultDto> SaveCheckpoint(Guid childId, Guid prcId, int recommendationId, bool completed)
    {
        _ = await _childService.GetAndValidateById(childId, prcId);

        var childBook = await _childBookRepository.GetByChildIdWithRecommendations(childId);

        if (childBook is null)
        {
            _logger.LogWarning("ChildBook for child {ChildId} not found", childId);
            throw new NotFoundException(_localizer["ChildBookNotFound"]);
        }

        var isActiveRecommendation = childBook.Recommendations.Any(r => r.Id == recommendationId);
        if (!isActiveRecommendation)
        {
            _logger.LogWarning("Recommendation {RecommendationId} not active for child {ChildId}", recommendationId, childId);
            throw new ValidationException(_localizer["RecommendationNotInChildBook"]);
        }

        var today = DateTime.UtcNow.Date;
        var existing = await _checkpointRepository.GetByDateAndIds(recommendationId, childBook.Id, today);
        if (existing is not null)
        {
            _logger.LogWarning("Checkpoint already exists for recommendation {RecommendationId} on {Date}", recommendationId, today);
            throw new ValidationException(_localizer["CheckpointAlreadyExists"]);
        }

        var checkpoint = new RecommendationCheckpoint(recommendationId, childBook.Id, completed);
        await _checkpointRepository.Create(checkpoint);

        if (completed)
        {
            var recommendation = childBook.Recommendations.First(r => r.Id == recommendationId);

            if (recommendation.IsAssimilated())
            {
                _logger.LogInformation(
                    "Recommendation {RecommendationId} assimilated for ChildBook {ChildBookId}",
                    recommendationId, childBook.Id);
                childBook.RemoveRecommendation(recommendationId);
                await _childBookRepository.Update(childBook);
                return new SaveCheckpointResultDto(assimilated: true);
            }
        }

        return new SaveCheckpointResultDto(assimilated: false);
    }
}
