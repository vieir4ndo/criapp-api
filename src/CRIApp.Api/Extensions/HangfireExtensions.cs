using System.Diagnostics.CodeAnalysis;
using Hangfire;
using Hangfire.PostgreSql;

namespace CRIApp.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class HangfireExtensions
{
    public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(cfg =>
            cfg.UsePostgreSqlStorage(c =>
                c.UseNpgsqlConnection(configuration.GetConnectionString("Postgres"))));

        services.AddHangfireServer();
        return services;
    }
}
