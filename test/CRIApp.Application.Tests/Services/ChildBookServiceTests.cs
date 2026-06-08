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
    {
        var childBook = new ChildBook(childId);
        return childBook;
    }

    private static Recommendation BuildRecommendation(int id)
    {
        var recommendation = new Recommendation("Do this.", Guid.NewGuid());
        typeof(Recommendation).GetProperty(nameof(Recommendation.Id))!.SetValue(recommendation, id);
        return recommendation;
    }

    private static Child BuildChild(Guid prcId, Guid childId)
    {
        var child = new Child("Child Name", new DateTime(2020, 1, 1), Sex.Male, 6, prcId, false);
        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(child, childId);
        return child;
    }

    private static RecommendationCheckpoint BuildCheckpoint(int recommendationId, Guid childBookId, bool completed)
    {
        var checkpoint = new RecommendationCheckpoint(recommendationId, childBookId, completed);
        return checkpoint;
    }
    
    [Fact]
    public async Task SaveCheckpoint_HappyPath_ShouldCreateCheckpoint()
    {
        var prcId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var child = BuildChild(prcId, childId);
        var childBook = BuildChildBook(childId);
        var recommendation = BuildRecommendation(5);
        typeof(ChildBook).GetProperty(nameof(ChildBook.Recommendations))!
            .SetValue(childBook, new List<Recommendation> { recommendation });

        _childServiceMock.Setup(s => s.GetAndValidateById(childId, prcId)).ReturnsAsync(child);
        _childBookRepositoryMock.Setup(r => r.GetByChildIdWithRecommendations(childId)).ReturnsAsync(childBook);
        _checkpointRepositoryMock
            .Setup(r => r.GetByDateAndIds(5, childBook.Id, It.IsAny<DateTime>()))
            .ReturnsAsync((RecommendationCheckpoint?)null);
        _checkpointRepositoryMock.Setup(r => r.Create(It.IsAny<RecommendationCheckpoint>())).Returns(Task.CompletedTask);
        _checkpointRepositoryMock
            .Setup(r => r.CountDistinctCompletedDaysInPeriod(5, childBook.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(1);

        await _sut.SaveCheckpoint(childId, prcId, 5, true);

        _checkpointRepositoryMock.Verify(r => r.Create(It.Is<RecommendationCheckpoint>(
            c => c.RecommendationId == 5 && c.ChildBookId == childBook.Id && c.Completed == true)), Times.Once);
        _childBookRepositoryMock.Verify(r => r.Update(It.IsAny<ChildBook>()), Times.Never);
    }

    [Fact]
    public async Task SaveCheckpoint_ChildBookNotFound_ShouldThrowNotFoundException()
    {
        var prcId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var child = BuildChild(prcId, childId);

        _childServiceMock.Setup(s => s.GetAndValidateById(childId, prcId)).ReturnsAsync(child);
        _childBookRepositoryMock.Setup(r => r.GetByChildId(childId)).ReturnsAsync((ChildBook?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.SaveCheckpoint(childId, prcId, 1, true));
    }

    [Fact]
    public async Task SaveCheckpoint_RecommendationNotInChildBook_ShouldThrowValidationException()
    {
        var prcId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var child = BuildChild(prcId, childId);
        var childBook = BuildChildBook(childId);
        typeof(ChildBook).GetProperty(nameof(ChildBook.Recommendations))!
            .SetValue(childBook, new List<Recommendation>());

        _childServiceMock.Setup(s => s.GetAndValidateById(childId, prcId)).ReturnsAsync(child);
        _childBookRepositoryMock.Setup(r => r.GetByChildIdWithRecommendations(childId)).ReturnsAsync(childBook);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.SaveCheckpoint(childId, prcId, 99, true));
    }

    [Fact]
    public async Task SaveCheckpoint_DuplicateDay_ShouldThrowValidationException()
    {
        var prcId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var child = BuildChild(prcId, childId);
        var childBook = BuildChildBook(childId);
        var recommendation = BuildRecommendation(3);
        typeof(ChildBook).GetProperty(nameof(ChildBook.Recommendations))!
            .SetValue(childBook, new List<Recommendation> { recommendation });

        var existingCheckpoint = BuildCheckpoint(3, childBook.Id, false);

        _childServiceMock.Setup(s => s.GetAndValidateById(childId, prcId)).ReturnsAsync(child);
        _childBookRepositoryMock.Setup(r => r.GetByChildIdWithRecommendations(childId)).ReturnsAsync(childBook);
        _checkpointRepositoryMock
            .Setup(r => r.GetByDateAndIds(3, childBook.Id, It.IsAny<DateTime>()))
            .ReturnsAsync(existingCheckpoint);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.SaveCheckpoint(childId, prcId, 3, true));

        _checkpointRepositoryMock.Verify(r => r.Create(It.IsAny<RecommendationCheckpoint>()), Times.Never);
    }

    [Fact]
    public async Task SaveCheckpoint_FourDistinctCompletedDays_ShouldAssimilateAndRemoveRecommendation()
    {
        var prcId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var child = BuildChild(prcId, childId);
        var childBook = BuildChildBook(childId);
        var recommendation = BuildRecommendation(7);
        typeof(ChildBook).GetProperty(nameof(ChildBook.Recommendations))!
            .SetValue(childBook, new List<Recommendation> { recommendation });

        _childServiceMock.Setup(s => s.GetAndValidateById(childId, prcId)).ReturnsAsync(child);
        _childBookRepositoryMock.Setup(r => r.GetByChildIdWithRecommendations(childId)).ReturnsAsync(childBook);
        _checkpointRepositoryMock
            .Setup(r => r.GetByDateAndIds(7, childBook.Id, It.IsAny<DateTime>()))
            .ReturnsAsync((RecommendationCheckpoint?)null);
        _checkpointRepositoryMock.Setup(r => r.Create(It.IsAny<RecommendationCheckpoint>())).Returns(Task.CompletedTask);
        _checkpointRepositoryMock
            .Setup(r => r.CountDistinctCompletedDaysInPeriod(7, childBook.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(4);
        _childBookRepositoryMock.Setup(r => r.Update(It.IsAny<ChildBook>())).Returns(Task.CompletedTask);

        await _sut.SaveCheckpoint(childId, prcId, 7, true);

        Assert.Empty(childBook.Recommendations);
        _childBookRepositoryMock.Verify(r => r.Update(childBook), Times.Once);
    }

    [Fact]
    public async Task SaveCheckpoint_FourDistinctDaysButCompletedFalse_ShouldNotAssimilate()
    {
        var prcId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var child = BuildChild(prcId, childId);
        var childBook = BuildChildBook(childId);
        var recommendation = BuildRecommendation(8);
        typeof(ChildBook).GetProperty(nameof(ChildBook.Recommendations))!
            .SetValue(childBook, new List<Recommendation> { recommendation });

        _childServiceMock.Setup(s => s.GetAndValidateById(childId, prcId)).ReturnsAsync(child);
        _childBookRepositoryMock.Setup(r => r.GetByChildIdWithRecommendations(childId)).ReturnsAsync(childBook);
        _checkpointRepositoryMock
            .Setup(r => r.GetByDateAndIds(8, childBook.Id, It.IsAny<DateTime>()))
            .ReturnsAsync((RecommendationCheckpoint?)null);
        _checkpointRepositoryMock.Setup(r => r.Create(It.IsAny<RecommendationCheckpoint>())).Returns(Task.CompletedTask);

        await _sut.SaveCheckpoint(childId, prcId, 8, false);

        Assert.Single(childBook.Recommendations);
        _checkpointRepositoryMock.Verify(
            r => r.CountDistinctCompletedDaysInPeriod(It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()),
            Times.Never);
        _childBookRepositoryMock.Verify(r => r.Update(It.IsAny<ChildBook>()), Times.Never);
    }
}
