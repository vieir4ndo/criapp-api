using CRIApp.Application.Dtos;
using CRIApp.Application.Services;
using CRIApp.Domain.Data;
using CRIApp.Domain.Entities;
using Moq;
using Xunit;

namespace CRIApp.Application.Tests.Services;

public class FaqServiceTests
{
    private readonly Mock<IFaqRepository> _repositoryMock;
    private readonly FaqService _sut;

    public FaqServiceTests()
    {
        _repositoryMock = new Mock<IFaqRepository>();
        _sut = new FaqService(_repositoryMock.Object);
    }

    private static Faq BuildFaq(string question = "What is this?", string answer = "An answer.")
        => new Faq(question, answer);

    [Fact]
    public async Task GetAll_WithFaqs_ShouldReturnMappedDtos()
    {
        var faqs = new List<Faq>
        {
            BuildFaq("Question 1?", "Answer 1."),
            BuildFaq("Question 2?", "Answer 2.")
        };
        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(faqs);

        var result = await _sut.GetAll();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, dto => dto.Question == "Question 1?");
        Assert.Contains(result, dto => dto.Question == "Question 2?");
    }

    [Fact]
    public async Task GetAll_NoFaqs_ShouldReturnEmptyList()
    {
        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Faq>());

        var result = await _sut.GetAll();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAll_ShouldMapAllFields()
    {
        var adminId = Guid.NewGuid();
        var faq = new Faq("How does it work?", "It works like this.");
        _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Faq> { faq });

        var result = await _sut.GetAll();

        var dto = result.Single();
        Assert.Equal(faq.Id, dto.Id);
        Assert.Equal("How does it work?", dto.Question);
        Assert.Equal("It works like this.", dto.Answer);
    }
}
