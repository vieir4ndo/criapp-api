using CRIApp.Api.Controllers.Responses;
using Xunit;

namespace CRIApp.Api.Tests;

public class ApiResponseTests
{
    [Fact]
    public void Constructor_WithSuccessAndEnumerable_ShouldSetData()
    {
        var data = new List<string> { "a", "b" };

        var response = new ApiResponse<string>(true, data);

        Assert.True(response.Success);
        Assert.Equal(2, response.Data.Count());
    }

    [Fact]
    public void Constructor_WithSuccessAndSingleItem_ShouldWrapInList()
    {
        var response = new ApiResponse<string>(true, "hello");

        Assert.True(response.Success);
        Assert.Single(response.Data);
        Assert.Equal("hello", response.Data.First());
    }

    [Fact]
    public void Constructor_WithMessage_ShouldSetFailureAndMessage()
    {
        var response = new ApiResponse<string>("something went wrong");

        Assert.False(response.Success);
        Assert.Single(response.Messages);
        Assert.Equal("something went wrong", response.Messages[0]);
    }

    [Fact]
    public void Constructor_WithNullMessage_ShouldSetFailureAndEmptyMessages()
    {
        var response = new ApiResponse<string>(null);

        Assert.False(response.Success);
        Assert.Empty(response.Messages);
    }

    [Fact]
    public void Constructor_WithSuccessOnly_ShouldHaveEmptyData()
    {
        var response = new ApiResponse<string>(true);

        Assert.True(response.Success);
        Assert.Empty(response.Data);
    }

    [Fact]
    public void Constructor_WithNullEnumerable_ShouldHaveEmptyData()
    {
        var response = new ApiResponse<string>(true, (IEnumerable<string>?)null);

        Assert.True(response.Success);
        Assert.Empty(response.Data);
    }
}
