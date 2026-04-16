using CRIApp.Api.Controllers;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using Moq;
using Xunit;

namespace CRIApp.Api.Tests.Controllers;

public class AudiovisualResourceControllerTests
{
    private readonly Mock<IAudiovisualResourceService> _serviceMock;
    private readonly AudiovisualResourceController _sut;

    public AudiovisualResourceControllerTests()
    {
        _serviceMock = new Mock<IAudiovisualResourceService>();
        _sut = new AudiovisualResourceController(_serviceMock.Object)
        {
            ControllerContext = ControllerContextFactory.CreateAuthenticated(Guid.NewGuid())
        };
    }

    private static AudiovisualResourceDto BuildDto(string name = "Video 1")
        => new AudiovisualResourceDto(
            new Domain.Entities.AudiovisualResource(name, "Description", "<iframe/>", Guid.NewGuid()));

    [Fact]
    public async Task GetAll_WithResources_ShouldReturnAllData()
    {
        var resources = new List<AudiovisualResourceDto>
        {
            BuildDto("Video 1"),
            BuildDto("Video 2")
        };
        _serviceMock.Setup(s => s.GetAll()).ReturnsAsync(resources);

        var result = await _sut.GetAll();

        Assert.True(result.Success);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task GetAll_NoResources_ShouldReturnEmptyData()
    {
        _serviceMock.Setup(s => s.GetAll()).ReturnsAsync(new List<AudiovisualResourceDto>());

        var result = await _sut.GetAll();

        Assert.True(result.Success);
        Assert.Empty(result.Data);
    }
}
