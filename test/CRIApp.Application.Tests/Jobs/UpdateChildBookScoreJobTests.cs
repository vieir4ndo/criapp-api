using CRIApp.Application.Jobs;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Reflection;
using Xunit;

namespace CRIApp.Application.Tests.Jobs;

public class UpdateChildBookScoreJobTests
{
    private readonly Mock<IChildBookRepository> _childBookRepositoryMock;
    private readonly UpdateChildBookScoreJob _sut;

    public UpdateChildBookScoreJobTests()
    {
        _childBookRepositoryMock = new Mock<IChildBookRepository>();
        _sut = new UpdateChildBookScoreJob(
            NullLogger<UpdateChildBookScoreJob>.Instance,
            _childBookRepositoryMock.Object);
    }

    private static ChildBook BuildChildBook()
    {
        var prcId = Guid.NewGuid();
        var child = new Child("Child", new DateTime(2020, 1, 1), Sex.Male, 6, prcId, false);
        typeof(Child).GetProperty(nameof(Child.Answers))!.SetValue(child, new List<Answer>());

        var childBook = new ChildBook(child.Id);
        typeof(ChildBook).GetProperty(nameof(ChildBook.Child))!.SetValue(childBook, child);
        return childBook;
    }

    [Fact]
    public async Task Run_NoChildBooks_ShouldNotCallUpdate()
    {
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook>());

        await _sut.Run();

        _childBookRepositoryMock.Verify(r => r.Update(It.IsAny<ChildBook>()), Times.Never);
    }

    [Fact]
    public async Task Run_WithChildBooks_ShouldCallUpdateForEach()
    {
        var childBooks = new List<ChildBook> { BuildChildBook(), BuildChildBook(), BuildChildBook() };
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(childBooks);

        await _sut.Run();

        _childBookRepositoryMock.Verify(r => r.Update(It.IsAny<ChildBook>()), Times.Exactly(3));
    }

    [Fact]
    public async Task Run_ShouldCallGetAllOnce()
    {
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { BuildChildBook() });

        await _sut.Run();

        _childBookRepositoryMock.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public async Task Run_ShouldCallUpdateWithCorrectChildBook()
    {
        var childBook = BuildChildBook();
        _childBookRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<ChildBook> { childBook });

        await _sut.Run();

        _childBookRepositoryMock.Verify(r => r.Update(childBook), Times.Once);
    }
}
