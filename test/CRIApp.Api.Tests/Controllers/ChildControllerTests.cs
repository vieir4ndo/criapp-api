using CRIApp.Api.Controllers;
using CRIApp.Api.Controllers.Requests;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using CRIApp.Domain.Entities;
using CRIApp.Domain.Enums;
using CRIApp.Domain.Exceptions;
using Moq;
using Xunit;

namespace CRIApp.Api.Tests.Controllers;

public class ChildControllerTests
{
    private readonly Mock<IChildService> _serviceMock;
    private readonly ChildController _sut;
    private readonly Guid _userId = Guid.NewGuid();

    public ChildControllerTests()
    {
        _serviceMock = new Mock<IChildService>();
        _sut = new ChildController(_serviceMock.Object)
        {
            ControllerContext = ControllerContextFactory.CreateAuthenticated(_userId)
        };
    }

    private static Child BuildChild(Guid prcId)
        => new Child("Alice", new DateTime(2020, 1, 1), Sex.Female, 6, prcId, false);

    [Fact]
    public async Task Create_ValidRequest_ShouldCallServiceAndReturnSuccess()
    {
        var request = new CreateChildRequest
        {
            Name = "Alice",
            BirthDate = "2020-03-10",
            Sex = Sex.Female,
            MonthsOfBreastFeeding = 6,
            IsPremature = false
        };
        _serviceMock.Setup(s => s.Create(It.IsAny<CreateChildDto>())).Returns(Task.CompletedTask);

        var result = await _sut.Create(request);

        Assert.True(result.Success);
        _serviceMock.Verify(s => s.Create(It.Is<CreateChildDto>(dto => dto.PrcId == _userId)), Times.Once);
    }

    [Fact]
    public async Task Create_EmptyName_ShouldThrowValidationException()
    {
        var request = new CreateChildRequest
        {
            Name = "",
            BirthDate = "2020-03-10",
            Sex = Sex.Female,
            MonthsOfBreastFeeding = 6,
            IsPremature = false
        };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Create(request));
    }

    [Fact]
    public async Task Create_InvalidDateFormat_ShouldThrowValidationException()
    {
        var request = new CreateChildRequest
        {
            Name = "Alice",
            BirthDate = "10-03-2020",
            Sex = Sex.Female,
            MonthsOfBreastFeeding = 6,
            IsPremature = false
        };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Create(request));
    }

    [Fact]
    public async Task Create_NegativeMonths_ShouldThrowValidationException()
    {
        var request = new CreateChildRequest
        {
            Name = "Alice",
            BirthDate = "2020-03-10",
            Sex = Sex.Female,
            MonthsOfBreastFeeding = -1,
            IsPremature = false
        };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Create(request));
    }

    [Fact]
    public async Task GetAllById_ShouldReturnChildrenForAuthenticatedUser()
    {
        var children = new List<ChildDto>
        {
            new ChildDto(BuildChild(_userId)),
            new ChildDto(BuildChild(_userId))
        };
        _serviceMock.Setup(s => s.GetAllById(_userId)).ReturnsAsync(children);

        var result = await _sut.GetAllById();

        Assert.True(result.Success);
        Assert.Equal(2, result.Data.Single().Count);
    }

    [Fact]
    public async Task Get_ValidChildId_ShouldReturnChildDto()
    {
        var childId = Guid.NewGuid();
        var child = BuildChild(_userId);
        _serviceMock.Setup(s => s.GetAndValidateById(childId, _userId)).ReturnsAsync(child);

        var result = await _sut.Get(childId);

        Assert.True(result.Success);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task Get_ChildNotFound_ShouldPropagateNotFoundException()
    {
        var childId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetAndValidateById(childId, _userId))
            .ThrowsAsync(new NotFoundException("Niño(a) no encontrado"));

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Get(childId));
    }

    [Fact]
    public async Task Update_ValidRequest_ShouldCallServiceAndReturnSuccess()
    {
        var childId = Guid.NewGuid();
        var request = new UpdateChildRequest { Name = "Updated Name" };
        _serviceMock.Setup(s => s.Update(_userId, childId, It.IsAny<UpdateChildDto>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.Update(request, childId);

        Assert.True(result.Success);
        _serviceMock.Verify(s => s.Update(_userId, childId, It.IsAny<UpdateChildDto>()), Times.Once);
    }

    [Fact]
    public async Task Update_EmptyName_ShouldThrowValidationException()
    {
        var request = new UpdateChildRequest { Name = "" };

        await Assert.ThrowsAsync<ValidationException>(() => _sut.Update(request, Guid.NewGuid()));
    }
}
