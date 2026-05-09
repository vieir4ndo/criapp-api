using System.Diagnostics.CodeAnalysis;
using CRIApp.Application.Jobs;
using Hangfire;

namespace CRIApp.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class JobsExtensions
{
    public static IServiceCollection AddJobServices(this IServiceCollection services)
    {
        services.AddTransient<NotifyUsersWithLessThanTenAnswersJob>();
        services.AddTransient<UpdateUsersScoreJob>();
        return services;
    }

    public static WebApplication UseRecurringJobs(this WebApplication app)
    {
        var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
        recurringJobManager.AddOrUpdate<NotifyUsersWithLessThanTenAnswersJob>(
            nameof(NotifyUsersWithLessThanTenAnswersJob),
            x => x.Run(),
            Cron.Daily);
        recurringJobManager.AddOrUpdate<UpdateUsersScoreJob>(
            nameof(UpdateUsersScoreJob),
            x => x.Run(),
            Cron.Minutely);
        return app;
    }
}
