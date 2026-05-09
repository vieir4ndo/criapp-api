using CRIApp.Domain.Entities;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Domain.Tests.Entities;

public class FaqTests
{
    private static readonly Guid ValidAdminId = Guid.NewGuid();

    private static Faq CreateValidFaq()
        => new Faq("What is CRI?", "CRI is a child rearing index.", ValidAdminId);

    [Fact]
    public void Create_ValidFaq_ShouldSucceed()
    {
        var faq = CreateValidFaq();

        Assert.NotNull(faq);
        Assert.Equal("What is CRI?", faq.Question);
        Assert.Equal("CRI is a child rearing index.", faq.Answer);
        Assert.Equal(ValidAdminId, faq.AdminId);
    }

    [Fact]
    public void Create_WithEmptyQuestion_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Faq("", "answer", ValidAdminId));
    }

    [Fact]
    public void Create_WithNullQuestion_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Faq(null!, "answer", ValidAdminId));
    }

    [Fact]
    public void Create_WithEmptyAnswer_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Faq("question?", "", ValidAdminId));
    }

    [Fact]
    public void Create_WithNullAnswer_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Faq("question?", null!, ValidAdminId));
    }

    [Fact]
    public void Create_WithEmptyAdminId_ShouldThrowDomainException()
    {
        Assert.Throws<DomainException>(() => new Faq("question?", "answer", Guid.Empty));
    }
}
