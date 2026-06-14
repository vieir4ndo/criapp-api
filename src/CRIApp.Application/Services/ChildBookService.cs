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

    public async Task<ChildBookDto> GetSummaryByChildBookId(Guid childBookId, Guid prcId)
    {
        var childBook = await _childBookRepository.GetById(childBookId);
        if (childBook is null)
        {
            _logger.LogWarning("ChildBook {ChildBookId} not found", childBookId);
            throw new NotFoundException(_localizer["ChildBookNotFound"]);
        }
        _ = await _childService.GetAndValidateById(childBook.ChildId, prcId);
        return new ChildBookDto(childBook);
    }

    public async Task<List<AwardDto>> GetAwardsByChildBookId(Guid childBookId, Guid prcId)
    {
        var childBook = await _childBookRepository.GetById(childBookId);
        if (childBook is null)
        {
            _logger.LogWarning("ChildBook {ChildBookId} not found", childBookId);
            throw new NotFoundException(_localizer["ChildBookNotFound"]);
        }
        _ = await _childService.GetAndValidateById(childBook.ChildId, prcId);
        return (await _childBookRepository.GetAwardsByChildBookId(childBookId))
            .Select(a => new AwardDto(a.Aw, a.AssignedAt)).ToList();
    }

    public async Task<List<RecommendationDto>> GetRecommendationsByChildBookId(Guid childBookId, Guid prcId)
    {
        var childBook = await _childBookRepository.GetById(childBookId);
        if (childBook is null)
        {
            _logger.LogWarning("ChildBook {ChildBookId} not found", childBookId);
            throw new NotFoundException(_localizer["ChildBookNotFound"]);
        }
        _ = await _childService.GetAndValidateById(childBook.ChildId, prcId);
        var recommendations = await _childBookRepository.GetRecommendationsByChildBookId(childBookId);
        return recommendations.Select(cbr => new RecommendationDto(cbr)).ToList();
    }

    public async Task<SaveCheckpointResultDto> SaveCheckpoint(Guid childBookId, Guid prcId, Guid childBookRecommendationId, bool completed)
    {
        var childBook = await _childBookRepository.GetByIdWithRecommendations(childBookId);

        if (childBook is null)
        {
            _logger.LogWarning("ChildBook {ChildBookId} not found", childBookId);
            throw new NotFoundException(_localizer["ChildBookNotFound"]);
        }

        _ = await _childService.GetAndValidateById(childBook.ChildId, prcId);

        var cbr = await _childBookRepository.GetRecommendationsByChildBookId(childBookId)
            .ContinueWith(t => t.Result.FirstOrDefault(x => x.Id == childBookRecommendationId));

        if (cbr is null)
        {
            _logger.LogWarning("ChildBookRecommendation {Id} not active for ChildBook {ChildBookId}", childBookRecommendationId, childBookId);
            throw new ValidationException(_localizer["RecommendationNotInChildBook"]);
        }

        var today = DateTime.UtcNow.Date;
        var existing = await _checkpointRepository.GetByDateAndIds(childBookRecommendationId, today);
        if (existing is not null)
        {
            _logger.LogWarning("Checkpoint already exists for ChildBookRecommendation {Id} on {Date}", childBookRecommendationId, today);
            throw new ValidationException(_localizer["CheckpointAlreadyExists"]);
        }

        var checkpoint = new RecommendationCheckpoint(childBookRecommendationId, completed);
        await _checkpointRepository.Create(checkpoint);

        if (completed)
        {
            cbr.Checkpoints.Add(checkpoint);

            if (cbr.IsAssimilated())
            {
                _logger.LogInformation(
                    "ChildBookRecommendation {Id} assimilated for ChildBook {ChildBookId}",
                    childBookRecommendationId, childBook.Id);
                childBook.RemoveRecommendation(cbr.RecommendationId);
                await _childBookRepository.Update(childBook);
                return new SaveCheckpointResultDto(assimilated: true);
            }
        }

        return new SaveCheckpointResultDto(assimilated: false);
    }
}
