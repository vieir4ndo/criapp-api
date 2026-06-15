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
    public void WithTranslations_ShouldSetTranslationFields()
    {
        var faq = CreateValidFaq();

        var result = faq.WithTranslations("What is CRI? (EN)", "CRI is an index. (EN)", "O que é CRI?", "CRI é um índice.");

        Assert.Same(faq, result);
        Assert.Equal("What is CRI? (EN)", faq.QuestionEn);
        Assert.Equal("CRI is an index. (EN)", faq.AnswerEn);
        Assert.Equal("O que é CRI?", faq.QuestionPt);
        Assert.Equal("CRI é um índice.", faq.AnswerPt);
    }

    [Fact]
    public void WithTranslations_NullValues_ShouldSetNulls()
    {
        var faq = CreateValidFaq();

        faq.WithTranslations(null, null, null, null);

        Assert.Null(faq.QuestionEn);
        Assert.Null(faq.AnswerEn);
        Assert.Null(faq.QuestionPt);
        Assert.Null(faq.AnswerPt);
    }

}
