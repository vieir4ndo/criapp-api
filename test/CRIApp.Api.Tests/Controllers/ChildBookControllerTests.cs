using CRIApp.Api.Controllers;
using CRIApp.Api.Controllers.Requests;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CRIApp.Api.Tests.Controllers;

public class ChildBookControllerTests
{
    private readonly Mock<IChildBookService> _serviceMock;
    private readonly Mock<ILogger<ChildBookController>> _loggerMock;
    private readonly ChildBookController _sut;
    private readonly Guid _userId = Guid.NewGuid();

    public ChildBookControllerTests()
    {
        _serviceMock = new Mock<IChildBookService>();
        _loggerMock = new Mock<ILogger<ChildBookController>>();
        _sut = new ChildBookController(_serviceMock.Object, _loggerMock.Object)
        {
            ControllerContext = ControllerContextFactory.CreateAuthenticated(_userId)
        };
    }

    [Fact]
    public async Task GetById_HappyPath_ShouldReturnSuccess()
    {
        var childBookId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var childBook = new ChildBook(childId);
        var dto = new ChildBookDto(childBook);
        _serviceMock.Setup(s => s.GetSummaryByChildBookId(childBookId, _userId)).ReturnsAsync(dto);

        var result = await _sut.GetById(childBookId);

        Assert.True(result.Success);
        Assert.Equal(childBook.ChildId, result.Data.Single().ChildId);
    }

    [Fact]
    public async Task GetById_NotFound_ShouldPropagate()
    {
        var childBookId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetSummaryByChildBookId(childBookId, _userId))
            .ThrowsAsync(new NotFoundException("not found"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetById(childBookId));
    }

    [Fact]
    public async Task GetAwards_HappyPath_ShouldReturnSuccess()
    {
        var childBookId = Guid.NewGuid();
        var award = new Award(AwardType.HabitBuilder, "Habit Builder");
        var awards = new List<AwardDto> { new AwardDto(award, DateTime.UtcNow) };
        _serviceMock.Setup(s => s.GetAwardsByChildBookId(childBookId, _userId)).ReturnsAsync(awards);

        var result = await _sut.GetAwards(childBookId);

        Assert.True(result.Success);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetRecommendations_HappyPath_ShouldReturnSuccess()
    {
        var childBookId = Guid.NewGuid();
        var cbr = new ChildBookRecommendation { Recommendation = new Recommendation("Do this", Guid.NewGuid()) };
        var recs = new List<RecommendationDto> { new RecommendationDto(cbr) };
        _serviceMock.Setup(s => s.GetRecommendationsByChildBookId(childBookId, _userId)).ReturnsAsync(recs);

        var result = await _sut.GetRecommendations(childBookId);

        Assert.True(result.Success);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task CreateCheckpoint_ValidRequest_ShouldReturnSuccess()
    {
        var childBookId = Guid.NewGuid();
        var cbrId = Guid.NewGuid();
        var request = new CreateCheckpointRequest { Completed = true };
        var resultDto = new SaveCheckpointResultDto(assimilated: false);
        _serviceMock.Setup(s => s.SaveCheckpoint(childBookId, _userId, cbrId, true)).ReturnsAsync(resultDto);

        var result = await _sut.CreateCheckpoint(childBookId, cbrId, request);

        Assert.True(result.Success);
        Assert.False(result.Data.Single().Assimilated);
    }

    [Fact]
    public async Task CreateCheckpoint_Assimilated_ShouldReturnAssimilatedTrue()
    {
        var childBookId = Guid.NewGuid();
        var cbrId = Guid.NewGuid();
        var request = new CreateCheckpointRequest { Completed = true };
        var resultDto = new SaveCheckpointResultDto(assimilated: true);
        _serviceMock.Setup(s => s.SaveCheckpoint(childBookId, _userId, cbrId, true)).ReturnsAsync(resultDto);

        var result = await _sut.CreateCheckpoint(childBookId, cbrId, request);

        Assert.True(result.Success);
        Assert.True(result.Data.Single().Assimilated);
    }

    [Fact]
    public async Task CreateCheckpoint_ValidationException_ShouldPropagate()
    {
        var childBookId = Guid.NewGuid();
        var cbrId = Guid.NewGuid();
        var request = new CreateCheckpointRequest { Completed = false };
        _serviceMock.Setup(s => s.SaveCheckpoint(childBookId, _userId, cbrId, false))
            .ThrowsAsync(new ValidationException("already exists"));

        await Assert.ThrowsAsync<ValidationException>(() => _sut.CreateCheckpoint(childBookId, cbrId, request));
    }
}
