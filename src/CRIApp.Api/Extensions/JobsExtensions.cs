using System.Diagnostics.CodeAnalysis;
using CRIApp.Application.Jobs;
using Hangfire;

namespace CRIApp.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class JobsExtensions
{
    public static IServiceCollection AddJobServices(this IServiceCollection services)
    {
        services.AddTransient<NotifyUsersWithLessThanTenAnswers>();
        return services;
    }

    public static WebApplication UseRecurringJobs(this WebApplication app)
    {
        var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
        recurringJobManager.AddOrUpdate<NotifyUsersWithLessThanTenAnswers>(
            nameof(NotifyUsersWithLessThanTenAnswers),
            x => x.Run(),
            Cron.Daily);

        return app;
    }
}
