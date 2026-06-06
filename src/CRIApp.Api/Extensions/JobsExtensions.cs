using System.Diagnostics.CodeAnalysis;
using CRIApp.Application.Jobs;
using Hangfire;

namespace CRIApp.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class JobsExtensions
{
    public static IServiceCollection AddJobServices(this IServiceCollection services)
    {
        services.AddTransient<SendMotivationalNotificationJob>();
        services.AddTransient<SendPointsRiskNotificationJob>();
        services.AddTransient<SendReEvaluationNotificationJob>();
        services.AddTransient<UpdateChildBookScoreJob>();
        services.AddTransient<AwardChildBookJob>();
        services.AddTransient<SaveUsersRecommendations>();
        services.AddTransient<CleanUserResetPasswordCodesJob>();
        services.AddTransient<CleanExpiredRefreshTokensJob>();
        return services;
    }

    public static WebApplication UseRecurringJobs(this WebApplication app)
    {
        var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

        recurringJobManager.RemoveIfExists(nameof(SendMotivationalNotificationJob));
        recurringJobManager.AddOrUpdate<SendMotivationalNotificationJob>(
            nameof(SendMotivationalNotificationJob),
            x => x.Run(),
            Cron.Daily);

        recurringJobManager.RemoveIfExists(nameof(SendPointsRiskNotificationJob));
        recurringJobManager.AddOrUpdate<SendPointsRiskNotificationJob>(
            nameof(SendPointsRiskNotificationJob),
            x => x.Run(),
            Cron.Daily);

        recurringJobManager.RemoveIfExists(nameof(SendReEvaluationNotificationJob));
        recurringJobManager.AddOrUpdate<SendReEvaluationNotificationJob>(
            nameof(SendReEvaluationNotificationJob),
            x => x.Run(),
            Cron.Daily);

        recurringJobManager.RemoveIfExists(nameof(UpdateChildBookScoreJob));
        recurringJobManager.AddOrUpdate<UpdateChildBookScoreJob>(
            nameof(UpdateChildBookScoreJob),
            x => x.Run(),
            "*/15 * * * *");

        recurringJobManager.RemoveIfExists(nameof(AwardChildBookJob));
        recurringJobManager.AddOrUpdate<AwardChildBookJob>(
            nameof(AwardChildBookJob),
            x => x.Run(),
            Cron.Daily);

        recurringJobManager.RemoveIfExists(nameof(SaveUsersRecommendations));
        recurringJobManager.AddOrUpdate<SaveUsersRecommendations>(
            nameof(SaveUsersRecommendations),
            x => x.Run(),
            Cron.Hourly);

        recurringJobManager.RemoveIfExists(nameof(CleanUserResetPasswordCodesJob));
        recurringJobManager.AddOrUpdate<CleanUserResetPasswordCodesJob>(
            nameof(CleanUserResetPasswordCodesJob),
            x => x.Run(),
            "*/10 * * * *");

        recurringJobManager.RemoveIfExists(nameof(CleanExpiredRefreshTokensJob));
        recurringJobManager.AddOrUpdate<CleanExpiredRefreshTokensJob>(
            nameof(CleanExpiredRefreshTokensJob),
            x => x.Run(),
            Cron.Daily);

        return app;
    }
}
