using System.Diagnostics.CodeAnalysis;
using CRIApp.Application.Abstractions;
using CRIApp.Application.Services;
using CRIApp.Domain.Data;
using CRIApp.Repository.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CRIApp.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<EmailTemplates>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<IAnswerService, AnswerService>();
        services.AddScoped<IChildService, ChildService>();
        services.AddScoped<ISmsClient, TwilioSmsClient>();
        services.AddScoped<IEmailClient, SmtpEmailClient>();
        services.AddScoped<IMessageUser, MessageService>();
        services.AddScoped<IFaqService, FaqService>();
        services.AddScoped<IForumService, ForumService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IAudiovisualResourceService, AudiovisualResourceService>();
        services.AddScoped<IChildBookService, ChildBookService>();
        services.AddScoped<INotificationService, NotificationService>();
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IAnswerRepository, AnswerRepository>();
        services.AddScoped<IChildRepository, ChildRepository>();
        services.AddScoped<IChildBookRepository, ChildBookRepository>();
        services.AddScoped<IUserResetPasswordCodeRepository, UserResetPasswordCodeRepository>();
        services.AddScoped<IFaqRepository, FaqRepository>();
        services.AddScoped<IForumRepository, ForumRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IAudiovisualResourceRepository, AudiovisualResourceRepository>();
        services.AddScoped<IAwardRepository, AwardRepository>();
        services.AddScoped<IRecommendationCheckpointRepository, RecommendationCheckpointRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
    }
}