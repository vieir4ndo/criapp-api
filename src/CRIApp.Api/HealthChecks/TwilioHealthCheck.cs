using CRIApp.Domain.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010;

namespace CRIApp.Api.HealthChecks;

public class TwilioHealthCheck : IHealthCheck
{
    private readonly TwilioSettings _settings;

    public TwilioHealthCheck(IOptions<TwilioSettings> settings)
    {
        _settings = settings.Value;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            TwilioClient.Init(_settings.ClientId, _settings.ClientSecret);
            var account = AccountResource.Fetch();
            return Task.FromResult(HealthCheckResult.Healthy($"Account: {account.FriendlyName}"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(ex.Message));
        }
    }
}
