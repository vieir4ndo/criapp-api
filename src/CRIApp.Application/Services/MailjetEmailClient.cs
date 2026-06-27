using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CRIApp.Application.Abstractions;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Options;

namespace CRIApp.Application.Services;

public class MailjetEmailClient : IEmailClient
{
    private readonly HttpClient _httpClient;
    private readonly MailjetSettings _settings;

    public MailjetEmailClient(HttpClient httpClient, IOptions<MailjetSettings> settings)
    {
        _settings = settings.Value;
        _httpClient = httpClient;
        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{_settings.ApiKey}:{_settings.ApiKeySecret}"));
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);
    }

    public async Task SendAsync(string from, string to, string subject, string body)
    {
        var payload = new
        {
            Messages = new[]
            {
                new
                {
                    From = new { Email = from },
                    To = new[] { new { Email = to } },
                    Subject = subject,
                    HTMLPart = body,
                    InlinedAttachments = new[]
                    {
                        new
                        {
                            ContentType = "image/png",
                            Filename = "logo.png",
                            Base64Content = EmailTemplates.LogoBase64,
                            ContentID = EmailTemplates.LogoContentId
                        }
                    }
                }
            }
        };

        var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        var json = JsonSerializer.Serialize(payload, options);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await _httpClient.PostAsync("/v3.1/send", content);
    }
}
