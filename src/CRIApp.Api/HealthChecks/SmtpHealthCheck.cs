using System.Net.Sockets;
using CRIApp.Domain.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace CRIApp.Api.HealthChecks;

public class SmtpHealthCheck : IHealthCheck
{
    private readonly MailjetSettings _settings;

    public SmtpHealthCheck(IOptions<MailjetSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(_settings.Host, _settings.Port, cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}
