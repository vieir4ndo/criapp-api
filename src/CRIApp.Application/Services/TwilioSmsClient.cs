using System.Net.Http.Headers;
using System.Text;
using CRIApp.Application.Abstractions;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Options;

namespace CRIApp.Application.Services;

public class TwilioSmsClient : ISmsClient
{
    private readonly HttpClient _httpClient;
    private readonly TwilioSettings _settings;

    public TwilioSmsClient(HttpClient httpClient, IOptions<TwilioSettings> settings)
    {
        _settings = settings.Value;
        _httpClient = httpClient;
        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);
    }

    public async Task SendAsync(string to, string from, string body)
    {
        var url = $"{_settings.BaseUrl}/2010-04-01/Accounts/{_settings.ClientId}/Messages.json";
        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("To", to),
            new KeyValuePair<string, string>("From", from),
            new KeyValuePair<string, string>("Body", body)
        ]);
        await _httpClient.PostAsync(url, content);
    }
}
