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
        services.AddTransient<UpdateChildBookScoreJob>();
        services.AddTransient<SaveUsersAdvices>();
        services.AddTransient<CalculateAndSaveAwards>();
        services.AddTransient<CleanUserResetPasswordCodesJob>();
        return services;
    }

    public static WebApplication UseRecurringJobs(this WebApplication app)
    {
        var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
        recurringJobManager.RemoveIfExists(nameof(NotifyUsersWithLessThanTenAnswersJob));
        recurringJobManager.AddOrUpdate<NotifyUsersWithLessThanTenAnswersJob>(
            nameof(NotifyUsersWithLessThanTenAnswersJob),
            x => x.Run(),
            Cron.Daily);
        recurringJobManager.RemoveIfExists(nameof(UpdateChildBookScoreJob));
        recurringJobManager.AddOrUpdate<UpdateChildBookScoreJob>(
            nameof(UpdateChildBookScoreJob),
            x => x.Run(),
            Cron.Minutely);
        recurringJobManager.RemoveIfExists(nameof(SaveUsersAdvices));
        recurringJobManager.AddOrUpdate<SaveUsersAdvices>(
            nameof(SaveUsersAdvices),
            x => x.Run(),
            Cron.Hourly);
        recurringJobManager.RemoveIfExists(nameof(CalculateAndSaveAwards));
        recurringJobManager.AddOrUpdate<CalculateAndSaveAwards>(
            nameof(CalculateAndSaveAwards),
            x => x.Run(),
            Cron.Hourly);
        recurringJobManager.RemoveIfExists(nameof(CleanUserResetPasswordCodesJob));
        recurringJobManager.AddOrUpdate<CleanUserResetPasswordCodesJob>(
            nameof(CleanUserResetPasswordCodesJob),
            x => x.Run(),
            Cron.Minutely);
        return app;
    }
}
