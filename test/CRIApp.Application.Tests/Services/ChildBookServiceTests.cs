using CRIApp.Application.Abstractions;
using CRIApp.Application.Resources;
using CRIApp.Application.Services;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Services;

public class ChildBookServiceTests
{
    private readonly Mock<IChildBookRepository> _childBookRepositoryMock;
    private readonly Mock<IRecommendationCheckpointRepository> _checkpointRepositoryMock;
    private readonly Mock<IChildService> _childServiceMock;
    private readonly ChildBookService _sut;

    public ChildBookServiceTests()
    {
        _childBookRepositoryMock = new Mock<IChildBookRepository>();
        _checkpointRepositoryMock = new Mock<IRecommendationCheckpointRepository>();
        _childServiceMock = new Mock<IChildService>();
        var localizerMock = new Mock<IStringLocalizer<AppMessages>>();
        _sut = new ChildBookService(
            _childBookRepositoryMock.Object,
            _checkpointRepositoryMock.Object,
            _childServiceMock.Object,
            NullLogger<ChildBookService>.Instance,
            localizerMock.Object);

        localizerMock.Setup(l => l[It.IsAny<string>()]).Returns<string>(k => new LocalizedString(k, k));
    }

    private static ChildBook BuildChildBook(Guid childId)
        => new ChildBook(childId);

    private static Child BuildChild(Guid prcId, Guid childId)
    {
        var child = new Child("Child Name", new DateTime(2020, 1, 1), Sex.Male, 6, prcId, false);
        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(child, childId);
        return child;
    }

    private static ChildBookRecommendation BuildCbr(Guid childBookId, int recommendationId)
    {
        var cbr = new ChildBookRecommendation
        {
            ChildBooksId = childBookId,
            RecommendationId = recommendationId,
            Recommendation = new Recommendation("Do this.", Guid.NewGuid())
        };
        typeof(Recommendation).GetProperty(nameof(Recommendation.Id))!.SetValue(cbr.Recommendation, recommendationId);
        return cbr;
    }

    private static RecommendationCheckpoint BuildCheckpointOnDate(Guid cbrId, Guid childBookId, bool completed, DateTime date)
    {
        var cp = new RecommendationCheckpoint(cbrId, completed);
        typeof(RecommendationCheckpoint)
            .GetProperty(nameof(RecommendationCheckpoint.Date))!
            .GetSetMethod(nonPublic: true)!
            .Invoke(cp, new object[] { date });
        return cp;
    }

    [Fact]
    public async Task SaveCheckpoint_HappyPath_ShouldCreateCheckpoint()
    {
        var prcId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var child = BuildChild(prcId, childId);
        var childBook = BuildChildBook(childId);
        var cbr = BuildCbr(childBook.Id, 5);

        _childServiceMock.Setup(s => s.GetAndValidateById(childId, prcId)).ReturnsAsync(child);
        _childBookRepositoryMock.Setup(r => r.GetByIdWithRecommendations(childBook.Id)).ReturnsAsync(childBook);
        _childBookRepositoryMock.Setup(r => r.GetRecommendationsByChildBookId(childBook.Id))
            .ReturnsAsync(new List<ChildBookRecommendation> { cbr });
        _checkpointRepositoryMock
            .Setup(r => r.GetByDateAndIds(cbr.Id, It.IsAny<DateTime>()))
            .ReturnsAsync((RecommendationCheckpoint?)null);
        _checkpointRepositoryMock.Setup(r => r.Create(It.IsAny<RecommendationCheckpoint>())).Returns(Task.CompletedTask);

        var result = await _sut.SaveCheckpoint(childBook.Id, prcId, cbr.Id, true);

        Assert.False(result.Assimilated);
        _checkpointRepositoryMock.Verify(r => r.Create(It.Is<RecommendationCheckpoint>(
            c => c.ChildBookRecommendationId == cbr.Id && c.Completed == true)), Times.Once);
        _childBookRepositoryMock.Verify(r => r.Update(It.IsAny<ChildBook>()), Times.Never);
    }

    [Fact]
    public async Task SaveCheckpoint_ChildBookNotFound_ShouldThrowNotFoundException()
    {
        var prcId = Guid.NewGuid();
        var childBookId = Guid.NewGuid();

        _childBookRepositoryMock.Setup(r => r.GetByIdWithRecommendations(childBookId)).ReturnsAsync((ChildBook?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.SaveCheckpoint(childBookId, prcId, Guid.NewGuid(), true));
    }

    [Fact]
    public async Task SaveCheckpoint_RecommendationNotInChildBook_ShouldThrowValidationException()
    {
        var prcId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var child = BuildChild(prcId, childId);
        var childBook = BuildChildBook(childId);

        _childServiceMock.Setup(s => s.GetAndValidateById(childId, prcId)).ReturnsAsync(child);
        _childBookRepositoryMock.Setup(r => r.GetByIdWithRecommendations(childBook.Id)).ReturnsAsync(childBook);
        _childBookRepositoryMock.Setup(r => r.GetRecommendationsByChildBookId(childBook.Id))
            .ReturnsAsync(new List<ChildBookRecommendation>());

        await Assert.ThrowsAsync<ValidationException>(() => _sut.SaveCheckpoint(childBook.Id, prcId, Guid.NewGuid(), true));
    }

    [Fact]
    public async Task SaveCheckpoint_DuplicateDay_ShouldThrowValidationException()
    {
        var prcId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var child = BuildChild(prcId, childId);
        var childBook = BuildChildBook(childId);
        var cbr = BuildCbr(childBook.Id, 3);
        var existingCheckpoint = new RecommendationCheckpoint(cbr.Id, false);

        _childServiceMock.Setup(s => s.GetAndValidateById(childId, prcId)).ReturnsAsync(child);
        _childBookRepositoryMock.Setup(r => r.GetByIdWithRecommendations(childBook.Id)).ReturnsAsync(childBook);
        _childBookRepositoryMock.Setup(r => r.GetRecommendationsByChildBookId(childBook.Id))
            .ReturnsAsync(new List<ChildBookRecommendation> { cbr });
        _checkpointRepositoryMock
            .Setup(r => r.GetByDateAndIds(cbr.Id, It.IsAny<DateTime>()))
            .ReturnsAsync(existingCheckpoint);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.SaveCheckpoint(childBook.Id, prcId, cbr.Id, true));

        _checkpointRepositoryMock.Verify(r => r.Create(It.IsAny<RecommendationCheckpoint>()), Times.Never);
    }

    [Fact]
    public async Task SaveCheckpoint_ThreeDistinctCompletedDays_ShouldAssimilateAndRemoveRecommendation()
    {
        var prcId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var child = BuildChild(prcId, childId);
        var childBook = BuildChildBook(childId);
        var cbr = BuildCbr(childBook.Id, 7);
        var today = DateTime.UtcNow.Date;
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, childBook.Id, true, today));
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, childBook.Id, true, today.AddDays(-1)));
        cbr.Checkpoints.Add(BuildCheckpointOnDate(cbr.Id, childBook.Id, true, today.AddDays(-2)));
        typeof(ChildBook).GetProperty(nameof(ChildBook.Recommendations))!
            .SetValue(childBook, new List<Recommendation> { cbr.Recommendation! });

        _childServiceMock.Setup(s => s.GetAndValidateById(childId, prcId)).ReturnsAsync(child);
        _childBookRepositoryMock.Setup(r => r.GetByIdWithRecommendations(childBook.Id)).ReturnsAsync(childBook);
        _childBookRepositoryMock.Setup(r => r.GetRecommendationsByChildBookId(childBook.Id))
            .ReturnsAsync(new List<ChildBookRecommendation> { cbr });
        _checkpointRepositoryMock
            .Setup(r => r.GetByDateAndIds(cbr.Id, It.IsAny<DateTime>()))
            .ReturnsAsync((RecommendationCheckpoint?)null);
        _checkpointRepositoryMock.Setup(r => r.Create(It.IsAny<RecommendationCheckpoint>())).Returns(Task.CompletedTask);
        _childBookRepositoryMock.Setup(r => r.Update(It.IsAny<ChildBook>())).Returns(Task.CompletedTask);

        var result = await _sut.SaveCheckpoint(childBook.Id, prcId, cbr.Id, true);

        Assert.True(result.Assimilated);
        Assert.Empty(childBook.Recommendations);
        _childBookRepositoryMock.Verify(r => r.Update(childBook), Times.Once);
    }

    [Fact]
    public async Task SaveCheckpoint_ThreeDistinctDaysButCompletedFalse_ShouldNotAssimilate()
    {
        var prcId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var child = BuildChild(prcId, childId);
        var childBook = BuildChildBook(childId);
        var cbr = BuildCbr(childBook.Id, 8);
        typeof(ChildBook).GetProperty(nameof(ChildBook.Recommendations))!
            .SetValue(childBook, new List<Recommendation> { cbr.Recommendation! });

        _childServiceMock.Setup(s => s.GetAndValidateById(childId, prcId)).ReturnsAsync(child);
        _childBookRepositoryMock.Setup(r => r.GetByIdWithRecommendations(childBook.Id)).ReturnsAsync(childBook);
        _childBookRepositoryMock.Setup(r => r.GetRecommendationsByChildBookId(childBook.Id))
            .ReturnsAsync(new List<ChildBookRecommendation> { cbr });
        _checkpointRepositoryMock
            .Setup(r => r.GetByDateAndIds(cbr.Id, It.IsAny<DateTime>()))
            .ReturnsAsync((RecommendationCheckpoint?)null);
        _checkpointRepositoryMock.Setup(r => r.Create(It.IsAny<RecommendationCheckpoint>())).Returns(Task.CompletedTask);

        var result = await _sut.SaveCheckpoint(childBook.Id, prcId, cbr.Id, false);

        Assert.False(result.Assimilated);
        Assert.Single(childBook.Recommendations);
        _childBookRepositoryMock.Verify(r => r.Update(It.IsAny<ChildBook>()), Times.Never);
    }

    [Fact]
    public async Task GetSummaryByChildBookId_HappyPath_ShouldReturnDto()
    {
        var prcId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var child = BuildChild(prcId, childId);
        var childBook = BuildChildBook(childId);
        typeof(ChildBook).GetProperty(nameof(ChildBook.Child))!.SetValue(childBook, child);

        _childBookRepositoryMock.Setup(r => r.GetById(childBook.Id)).ReturnsAsync(childBook);
        _childServiceMock.Setup(s => s.GetAndValidateById(childId, prcId)).ReturnsAsync(child);

        var result = await _sut.GetSummaryByChildBookId(childBook.Id, prcId);

        Assert.Equal(childBook.Id, result.Id);
        Assert.Equal(childBook.ChildId, result.ChildId);
    }

    [Fact]
    public async Task GetSummaryByChildBookId_NotFound_ShouldThrowNotFoundException()
    {
        var prcId = Guid.NewGuid();
        var childBookId = Guid.NewGuid();
        _childBookRepositoryMock.Setup(r => r.GetById(childBookId)).ReturnsAsync((ChildBook?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetSummaryByChildBookId(childBookId, prcId));
    }

    [Fact]
    public async Task GetAwardsByChildBookId_HappyPath_ShouldReturnAwards()
    {
        var prcId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var child = BuildChild(prcId, childId);
        var childBook = BuildChildBook(childId);

        var award = new Award(AwardType.HabitBuilder, "Habit Builder");
        _childBookRepositoryMock.Setup(r => r.GetById(childBook.Id)).ReturnsAsync(childBook);
        _childServiceMock.Setup(s => s.GetAndValidateById(childId, prcId)).ReturnsAsync(child);
        _childBookRepositoryMock
            .Setup(r => r.GetAwardsByChildBookId(childBook.Id))
            .ReturnsAsync(new List<(Award Aw, DateTime AssignedAt)> { (award, DateTime.UtcNow) });

        var result = await _sut.GetAwardsByChildBookId(childBook.Id, prcId);

        Assert.Single(result);
        Assert.Equal(AwardType.HabitBuilder, result[0].Type);
    }

    [Fact]
    public async Task GetAwardsByChildBookId_NotFound_ShouldThrowNotFoundException()
    {
        var prcId = Guid.NewGuid();
        var childBookId = Guid.NewGuid();
        _childBookRepositoryMock.Setup(r => r.GetById(childBookId)).ReturnsAsync((ChildBook?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetAwardsByChildBookId(childBookId, prcId));
    }

    [Fact]
    public async Task GetRecommendationsByChildBookId_HappyPath_ShouldReturnRecommendations()
    {
        var prcId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var child = BuildChild(prcId, childId);
        var childBook = BuildChildBook(childId);
        var cbr = BuildCbr(childBook.Id, 1);

        _childBookRepositoryMock.Setup(r => r.GetById(childBook.Id)).ReturnsAsync(childBook);
        _childServiceMock.Setup(s => s.GetAndValidateById(childId, prcId)).ReturnsAsync(child);
        _childBookRepositoryMock
            .Setup(r => r.GetRecommendationsByChildBookId(childBook.Id))
            .ReturnsAsync(new List<ChildBookRecommendation> { cbr });

        var result = await _sut.GetRecommendationsByChildBookId(childBook.Id, prcId);

        Assert.Single(result);
        Assert.Equal(cbr.Id, result[0].ChildBookRecommendationId);
    }

    [Fact]
    public async Task GetRecommendationsByChildBookId_NotFound_ShouldThrowNotFoundException()
    {
        var prcId = Guid.NewGuid();
        var childBookId = Guid.NewGuid();
        _childBookRepositoryMock.Setup(r => r.GetById(childBookId)).ReturnsAsync((ChildBook?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetRecommendationsByChildBookId(childBookId, prcId));
    }
}
