using Microsoft.Extensions.Logging;
using CRIApp.Api.Controllers;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Dtos;
using Moq;
using Xunit;

namespace CRIApp.Api.Tests.Controllers;

public class FaqControllerTests
{
    private readonly Mock<IFaqService> _serviceMock;
    private readonly Mock<ILogger<FaqController>> _loggerMock;
    private readonly FaqController _sut;

    public FaqControllerTests()
    {
        _serviceMock = new Mock<IFaqService>();
        _loggerMock = new Mock<ILogger<FaqController>>();
        _sut = new FaqController(_serviceMock.Object, _loggerMock.Object)
        {
            ControllerContext = ControllerContextFactory.CreateAuthenticated(Guid.NewGuid())
        };
    }

    private static FaqDto BuildFaqDto(string question = "Q?", string answer = "A.")
        => new FaqDto(new Domain.Entities.Faq(question, answer));

    [Fact]
    public async Task GetAll_WithFaqs_ShouldReturnAllData()
    {
        var faqs = new List<FaqDto>
        {
            BuildFaqDto("Question 1?", "Answer 1."),
            BuildFaqDto("Question 2?", "Answer 2.")
        };
        _serviceMock.Setup(s => s.GetAll()).ReturnsAsync(faqs);

        var result = await _sut.GetAll();

        Assert.True(result.Success);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task GetAll_NoFaqs_ShouldReturnEmptyData()
    {
        _serviceMock.Setup(s => s.GetAll()).ReturnsAsync(new List<FaqDto>());

        var result = await _sut.GetAll();

        Assert.True(result.Success);
        Assert.Empty(result.Data);
    }
}
