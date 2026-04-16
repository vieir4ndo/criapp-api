using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Application.Services;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Services;

public class ChildServiceTests
{
    private readonly Mock<IChildRepository> _childRepositoryMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly ChildService _sut;

    public ChildServiceTests()
    {
        _childRepositoryMock = new Mock<IChildRepository>();
        _userServiceMock = new Mock<IUserService>();
        _sut = new ChildService(_childRepositoryMock.Object, _userServiceMock.Object);
    }

    private static User BuildUser()
        => new User("Parent", new DateTime(1985, 5, 10), Sex.Female, "parent@domain.com", "pass123");

    private static Child BuildChild(Guid prcId)
        => new Child("Child Name", new DateTime(2020, 1, 1), Sex.Male, 6, prcId, false);

    [Fact]
    public async Task Create_ValidDto_ShouldCreateChild()
    {
        var prcId = Guid.NewGuid();
        var user = BuildUser();
        var dto = new CreateChildDto
        {
            Name = "Alice",
            BirthDate = new DateTime(2021, 3, 10),
            Sex = Sex.Female,
            MonthsOfBreastFeeding = 3,
            PrcId = prcId,
            IsPremature = false
        };

        _userServiceMock.Setup(s => s.GetAndValidateById(prcId)).ReturnsAsync(user);
        _childRepositoryMock.Setup(r => r.Create(It.IsAny<Child>())).Returns(Task.CompletedTask);

        await _sut.Create(dto);

        _childRepositoryMock.Verify(r => r.Create(It.IsAny<Child>()), Times.Once);
    }

    [Fact]
    public async Task Create_UserNotFound_ShouldThrowNotFoundException()
    {
        var prcId = Guid.NewGuid();
        var dto = new CreateChildDto { PrcId = prcId, Name = "Alice", BirthDate = new DateTime(2021, 3, 10), Sex = Sex.Female };

        _userServiceMock.Setup(s => s.GetAndValidateById(prcId))
            .ThrowsAsync(new NotFoundException("Usuario no encontrado"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Create(dto));
    }

    [Fact]
    public async Task GetAllById_ShouldReturnMappedChildDtos()
    {
        var prcId = Guid.NewGuid();
        var children = new List<Child>
        {
            BuildChild(prcId),
            BuildChild(prcId)
        };
        _childRepositoryMock.Setup(r => r.GetAllById(prcId)).ReturnsAsync(children);

        var result = await _sut.GetAllById(prcId);

        Assert.Equal(2, result.Count);
        Assert.All(result, dto => Assert.Equal(prcId, dto.PrcId));
    }

    [Fact]
    public async Task GetAllById_NoneFound_ShouldReturnEmptyList()
    {
        var prcId = Guid.NewGuid();
        _childRepositoryMock.Setup(r => r.GetAllById(prcId)).ReturnsAsync(new List<Child>());

        var result = await _sut.GetAllById(prcId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAndValidateById_ChildNotFound_ShouldThrowNotFoundException()
    {
        var childId = Guid.NewGuid();
        var prcId = Guid.NewGuid();
        _childRepositoryMock.Setup(r => r.GetById(childId)).ReturnsAsync((Child?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetAndValidateById(childId, prcId));
    }

    [Fact]
    public async Task GetAndValidateById_ChildBelongsToAnotherUser_ShouldThrowValidationException()
    {
        var childId = Guid.NewGuid();
        var prcId = Guid.NewGuid();
        var otherPrcId = Guid.NewGuid();
        var child = BuildChild(otherPrcId);

        _childRepositoryMock.Setup(r => r.GetById(childId)).ReturnsAsync(child);

        await Assert.ThrowsAsync<ValidationException>(() => _sut.GetAndValidateById(childId, prcId));
    }

    [Fact]
    public async Task GetAndValidateById_ValidOwnership_ShouldReturnChild()
    {
        var childId = Guid.NewGuid();
        var prcId = Guid.NewGuid();
        var child = BuildChild(prcId);

        _childRepositoryMock.Setup(r => r.GetById(childId)).ReturnsAsync(child);

        var result = await _sut.GetAndValidateById(childId, prcId);

        Assert.Equal(child, result);
    }

    [Fact]
    public async Task Update_ValidData_ShouldCallUpdateOnRepository()
    {
        var prcId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var user = BuildUser();
        var child = BuildChild(prcId);
        var dto = new UpdateChildDto { Name = "Updated Name" };

        _userServiceMock.Setup(s => s.GetAndValidateById(prcId)).ReturnsAsync(user);
        _childRepositoryMock.Setup(r => r.GetById(childId)).ReturnsAsync(child);
        _childRepositoryMock.Setup(r => r.Update(It.IsAny<Child>())).Returns(Task.CompletedTask);

        await _sut.Update(prcId, childId, dto);

        _childRepositoryMock.Verify(r => r.Update(child), Times.Once);
    }

    [Fact]
    public async Task Update_UserNotFound_ShouldThrowNotFoundException()
    {
        var prcId = Guid.NewGuid();
        _userServiceMock.Setup(s => s.GetAndValidateById(prcId))
            .ThrowsAsync(new NotFoundException("Usuario no encontrado"));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _sut.Update(prcId, Guid.NewGuid(), new UpdateChildDto()));
    }
}
