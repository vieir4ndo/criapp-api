using CRIApp.Domain.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace CRIApp.Api.HealthChecks;

public class MailjetHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;

    public MailjetHealthCheck(IHttpClientFactory httpClientFactory, IOptions<MailjetSettings> settings)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(settings.Value.BaseUrl);
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/", cancellationToken);
            return (int)response.StatusCode < 500
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy($"Mailjet API returned {(int)response.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}
