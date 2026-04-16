using CRIApp.Application.Services;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Services;

public class AudiovisualResourceServiceTests
{
    private readonly Mock<IAudiovisualResourceRepository> _repositoryMock;
    private readonly AudiovisualResourceService _sut;

    public AudiovisualResourceServiceTests()
    {
        _repositoryMock = new Mock<IAudiovisualResourceRepository>();
        _sut = new AudiovisualResourceService(_repositoryMock.Object);
    }

    private static AudiovisualResource BuildResource(string name = "Video 1")
        => new AudiovisualResource(name, "A description", "<iframe/>", Guid.NewGuid());

    [Fact]
    public async Task GetAll_WithResources_ShouldReturnMappedDtos()
    {
        var resources = new List<AudiovisualResource>
        {
            BuildResource("Video 1"),
            BuildResource("Video 2")
        };
        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(resources);

        var result = await _sut.GetAll();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, dto => dto.Name == "Video 1");
        Assert.Contains(result, dto => dto.Name == "Video 2");
    }

    [Fact]
    public async Task GetAll_NoResources_ShouldReturnEmptyList()
    {
        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<AudiovisualResource>());

        var result = await _sut.GetAll();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAll_ShouldMapAllFields()
    {
        var adminId = Guid.NewGuid();
        var resource = new AudiovisualResource("Tutorial", "Learn CRI", "<iframe src='url'/>", adminId);
        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<AudiovisualResource> { resource });

        var result = await _sut.GetAll();

        var dto = result.Single();
        Assert.Equal(resource.Id, dto.Id);
        Assert.Equal("Tutorial", dto.Name);
        Assert.Equal("Learn CRI", dto.Description);
        Assert.Equal("<iframe src='url'/>", dto.Iframe);
    }
}
