using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Application.Resources;
using CRIApp.Domain.Data;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace CRIApp.Application.Services;

public class ChildBookService : IChildBookService
{
    private readonly IChildBookRepository _childBookRepository;
    private readonly IChildService _childService;
    private readonly ILogger<ChildBookService> _logger;
    private readonly IStringLocalizer<AppMessages> _localizer;

    public ChildBookService(
        IChildBookRepository childBookRepository,
        IChildService childService,
        ILogger<ChildBookService> logger,
        IStringLocalizer<AppMessages> localizer)
    {
        _childBookRepository = childBookRepository;
        _childService = childService;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task<ChildBookDto> GetByChildId(Guid childId, Guid prcId)
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
}
