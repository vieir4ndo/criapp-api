using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Domain.Tests.Exceptions;

public class ExceptionTests
{
    [Fact]
    public void DomainException_WithStringMessage_ShouldExposeError()
    {
        var ex = new DomainException("single error");

        Assert.Single(ex.Errors);
        Assert.Equal("single error", ex.Errors[0]);
    }

    [Fact]
    public void DomainException_WithListOfMessages_ShouldExposeAllErrors()
    {
        var errors = new List<string> { "error 1", "error 2" };

        var ex = new DomainException(errors);

        Assert.Equal(2, ex.Errors.Count);
        Assert.Contains("error 1", ex.Errors);
        Assert.Contains("error 2", ex.Errors);
    }

    [Fact]
    public void NotFoundException_WithStringMessage_ShouldExposeError()
    {
        var ex = new NotFoundException("not found");

        Assert.Single(ex.Errors);
        Assert.Equal("not found", ex.Errors[0]);
    }

    [Fact]
    public void NotFoundException_WithListOfMessages_ShouldExposeAllErrors()
    {
        var errors = new List<string> { "resource 1 not found", "resource 2 not found" };

        var ex = new NotFoundException(errors);

        Assert.Equal(2, ex.Errors.Count);
    }

    [Fact]
    public void ValidationException_WithStringMessage_ShouldExposeError()
    {
        var ex = new ValidationException("invalid input");

        Assert.Single(ex.Errors);
        Assert.Equal("invalid input", ex.Errors[0]);
    }

    [Fact]
    public void ValidationException_WithListOfMessages_ShouldExposeAllErrors()
    {
        var errors = new List<string> { "field A required", "field B invalid" };

        var ex = new ValidationException(errors);

        Assert.Equal(2, ex.Errors.Count);
        Assert.Contains("field A required", ex.Errors);
    }

    [Fact]
    public void DomainException_IsBaseException()
    {
        var ex = new DomainException("error");

        Assert.IsAssignableFrom<BaseException>(ex);
    }

    [Fact]
    public void NotFoundException_IsBaseException()
    {
        var ex = new NotFoundException("error");

        Assert.IsAssignableFrom<BaseException>(ex);
    }

    [Fact]
    public void ValidationException_IsBaseException()
    {
        var ex = new ValidationException("error");

        Assert.IsAssignableFrom<BaseException>(ex);
    }
}
