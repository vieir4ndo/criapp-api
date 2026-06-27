using System.Net;
using System.Text;
using System.Text.Json;
using CRIApp.Application.Services;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Options;
using Xunit;

namespace CRIApp.Application.Tests.Services;

public class MailjetEmailClientTests
{
    private sealed class FakeHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public string LastRequestBody { get; private set; } = string.Empty;
        public HttpStatusCode ResponseStatus { get; set; } = HttpStatusCode.OK;

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            if (request.Content is not null)
                LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            return new HttpResponseMessage(ResponseStatus);
        }
    }

    private static (MailjetEmailClient client, FakeHandler handler) Build(
        string baseUrl = "https://fake.mailjet.com",
        string apiKey = "test-key",
        string apiKeySecret = "test-secret")
    {
        var handler = new FakeHandler();
        var httpClient = new HttpClient(handler);
        var settings = Options.Create(new MailjetSettings
        {
            BaseUrl = baseUrl,
            ApiKey = apiKey,
            ApiKeySecret = apiKeySecret,
            SenderEmail = "no-reply@criapp.com"
        });
        return (new MailjetEmailClient(httpClient, settings), handler);
    }

    [Fact]
    public async Task SendAsync_ShouldPostToCorrectEndpoint()
    {
        var (client, handler) = Build();

        await client.SendAsync("from@criapp.com", "to@test.com", "Subject", "<p>Body</p>");

        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest.Method);
        Assert.Equal("/v3.1/send", handler.LastRequest.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task SendAsync_ShouldSendBasicAuthHeader()
    {
        var (client, handler) = Build(apiKey: "mykey", apiKeySecret: "mysecret");

        await client.SendAsync("from@criapp.com", "to@test.com", "Subject", "<p>Body</p>");

        var auth = handler.LastRequest!.Headers.Authorization;
        Assert.NotNull(auth);
        Assert.Equal("Basic", auth.Scheme);
        var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(auth.Parameter!));
        Assert.Equal("mykey:mysecret", decoded);
    }

    [Fact]
    public async Task SendAsync_ShouldIncludeFromToSubjectAndHtmlPart()
    {
        var (client, handler) = Build();

        await client.SendAsync("from@criapp.com", "to@test.com", "Hello", "<p>World</p>");

        using var doc = JsonDocument.Parse(handler.LastRequestBody);
        var msg = doc.RootElement.GetProperty("Messages")[0];

        Assert.Equal("from@criapp.com", msg.GetProperty("From").GetProperty("Email").GetString());
        Assert.Equal("to@test.com", msg.GetProperty("To")[0].GetProperty("Email").GetString());
        Assert.Equal("Hello", msg.GetProperty("Subject").GetString());
        Assert.Equal("<p>World</p>", msg.GetProperty("HTMLPart").GetString());
    }

    [Fact]
    public async Task SendAsync_ShouldAttachInlinedLogo()
    {
        var (client, handler) = Build();

        await client.SendAsync("from@criapp.com", "to@test.com", "Subject", "<p>Body</p>");

        using var doc = JsonDocument.Parse(handler.LastRequestBody);
        var attachments = doc.RootElement.GetProperty("Messages")[0].GetProperty("InlinedAttachments");

        Assert.Equal(1, attachments.GetArrayLength());
        Assert.Equal("image/png", attachments[0].GetProperty("ContentType").GetString());
        Assert.Equal("criapp-logo", attachments[0].GetProperty("ContentID").GetString());
    }
}
