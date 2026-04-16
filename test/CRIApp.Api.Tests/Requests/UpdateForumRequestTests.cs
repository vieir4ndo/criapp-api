using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Api.Tests.Requests;

public class UpdateForumRequestTests
{
    [Fact]
    public void AllNullFields_ShouldNotThrow()
    {
        var request = new UpdateForumRequest();

        var ex = Record.Exception(() => request.ValidateAndThrow());

        Assert.Null(ex);
    }

    [Fact]
    public void ValidPartialUpdate_Name_ShouldNotThrow()
    {
        var request = new UpdateForumRequest { Name = "New Name" };

        var ex = Record.Exception(() => request.ValidateAndThrow());

        Assert.Null(ex);
    }

    [Fact]
    public void ValidPartialUpdate_Description_ShouldNotThrow()
    {
        var request = new UpdateForumRequest { Description = "New Description" };

        var ex = Record.Exception(() => request.ValidateAndThrow());

        Assert.Null(ex);
    }

    [Fact]
    public void EmptyName_WhenProvided_ShouldThrowValidationException()
    {
        var request = new UpdateForumRequest { Name = "" };

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }

    [Fact]
    public void EmptyDescription_WhenProvided_ShouldThrowValidationException()
    {
        var request = new UpdateForumRequest { Description = "" };

        Assert.Throws<ValidationException>(() => request.ValidateAndThrow());
    }
}
