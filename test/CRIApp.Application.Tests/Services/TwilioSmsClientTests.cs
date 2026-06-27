using System.Net;
using System.Text;
using System.Web;
using CRIApp.Application.Services;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Options;
using Xunit;

namespace CRIApp.Application.Tests.Services;

public class TwilioSmsClientTests
{
    private sealed class FakeHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public string LastRequestBody { get; private set; } = string.Empty;
        public HttpStatusCode ResponseStatus { get; set; } = HttpStatusCode.Created;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            if (request.Content is not null)
                LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(ResponseStatus);
        }
    }

    private static (TwilioSmsClient client, FakeHandler handler) Build(
        string baseUrl = "https://fake.twilio.com",
        string clientId = "ACTEST123",
        string clientSecret = "test-secret")
    {
        var handler = new FakeHandler();
        var httpClient = new HttpClient(handler);
        var settings = Options.Create(new TwilioSettings
        {
            BaseUrl = baseUrl,
            ClientId = clientId,
            ClientSecret = clientSecret,
            SenderPhoneNumber = "+34600000000"
        });
        return (new TwilioSmsClient(httpClient, settings), handler);
    }

    [Fact]
    public async Task SendAsync_ShouldPostToCorrectEndpoint()
    {
        var (client, handler) = Build(clientId: "ACTEST123");

        await client.SendAsync("+34611111111", "+34600000000", "Test message");

        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Contains("/2010-04-01/Accounts/ACTEST123/Messages.json",
            handler.LastRequest.RequestUri!.ToString());
    }

    [Fact]
    public async Task SendAsync_ShouldSendBasicAuthHeader()
    {
        var (client, handler) = Build(clientId: "ACID", clientSecret: "ACSECRET");

        await client.SendAsync("+34611111111", "+34600000000", "Test message");

        var auth = handler.LastRequest!.Headers.Authorization;
        Assert.NotNull(auth);
        Assert.Equal("Basic", auth.Scheme);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(auth.Parameter!));
        Assert.Equal("ACID:ACSECRET", decoded);
    }

    [Fact]
    public async Task SendAsync_ShouldIncludeToFromAndBodyInForm()
    {
        var (client, handler) = Build();

        await client.SendAsync("+34611111111", "+34600000000", "Hello CRIApp");

        var form = HttpUtility.ParseQueryString(handler.LastRequestBody);
        Assert.Equal("+34611111111", form["To"]);
        Assert.Equal("+34600000000", form["From"]);
        Assert.Equal("Hello CRIApp", form["Body"]);
    }
}
