using CRIApp.Api.Controllers.Requests;
using CRIApp.Domain.Exceptions;
using Xunit;

namespace CRIApp.Api.Tests.Requests;

public class CreateCheckpointRequestTests
{
    [Fact]
    public void ValidRequest_Completed_True_ShouldNotThrow()
    {
        var request = new CreateCheckpointRequest { Completed = true };

        var ex = Record.Exception(() => request.ValidateAndThrow());

        Assert.Null(ex);
    }

    [Fact]
    public void ValidRequest_Completed_False_ShouldNotThrow()
    {
        var request = new CreateCheckpointRequest { Completed = false };

        var ex = Record.Exception(() => request.ValidateAndThrow());

        Assert.Null(ex);
    }
}
