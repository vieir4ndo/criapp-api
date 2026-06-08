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

        var childBook = await _childBookRepository.GetByChildId(childId);

        if (childBook is null)
        {
            _logger.LogWarning("ChildBook for child {ChildId} not found", childId);
            throw new NotFoundException(_localizer["ChildBookNotFound"]);
        }

        var today = DateTime.UtcNow.Date;
        var windowStart = today.AddDays(-6);
        var todayCheckpoints = await _checkpointRepository.GetByChildBookIdAndDate(childBook.Id, today);
        var completedCountMap = await _checkpointRepository.GetDistinctCompletedDaysInWindowByChildBook(childBook.Id, windowStart, today);
        var recommendations = await _childBookRepository.GetRecommendationsByChildBookId(childBook.Id);

        var checkpointMap = todayCheckpoints.ToDictionary(c => c.RecommendationId, c => (bool?)c.Completed);
        return recommendations.Select(tuple =>
        {
            checkpointMap.TryGetValue(tuple.Rec.Id, out var todayCompleted);
            completedCountMap.TryGetValue(tuple.Rec.Id, out var completedCount);
            return new RecommendationDto(tuple.Rec, todayCompleted, tuple.AssignedAt, completedCount);
        }).ToList();
    }
    
    public async Task SaveCheckpoint(Guid childId, Guid prcId, int recommendationId, bool completed)
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
            var windowStart = today.AddDays(-6);
            var distinctDays = await _checkpointRepository.CountDistinctCompletedDaysInPeriod(
                recommendationId, childBook.Id, windowStart, today);

            if (distinctDays >= 4)
            {
                _logger.LogInformation(
                    "Recommendation {RecommendationId} assimilated for ChildBook {ChildBookId}",
                    recommendationId, childBook.Id);
                childBook.RemoveRecommendation(recommendationId);
                await _childBookRepository.Update(childBook);
            }
        }
    }
}
