using System.Text.Json;
using CRIApp.Api.HealthChecks;
using CRIApp.Api.Middlewares;
using CRIApp.Application.Extensions;
using CRIApp.Application.Jobs;
using CRIApp.Domain.Settings;
using CRIApp.Repository.Seeders;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using AppContext = CRIApp.Repository.AppContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

// TODO: Isolate in extensions
var jwtSettingsConfigurationSection = builder.Configuration.GetSection(JwtTokenSettings.SectionName);
builder.Services.Configure<JwtTokenSettings>(jwtSettingsConfigurationSection);
var jwtSettings = jwtSettingsConfigurationSection.Get<JwtTokenSettings>();
if (jwtSettings == null)
{
    throw new ArgumentNullException(nameof(jwtSettings) + "was not found in the configuration file.");
}
var twilioSettings = builder.Configuration.GetSection(TwilioSettings.SectionName);
builder.Services.Configure<TwilioSettings>(twilioSettings);
var mailjetSettings = builder.Configuration.GetSection(MailjetSettings.SectionName);
builder.Services.Configure<MailjetSettings>(mailjetSettings);
// HF
builder.Services.AddHangfire(configuration =>
{
    configuration.UsePostgreSqlStorage(c =>
        c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("Postgres")));
});
builder.Services.AddHangfireServer();
// HF - End

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });
//Jwt configuration ends here

// Set the default language for FluentValidation error messages
ValidatorOptions.Global.LanguageManager.Enabled = true;
ValidatorOptions.Global.LanguageManager.Culture = new System.Globalization.CultureInfo("es-ES");

// Add services to the container.
builder.Services.AddDbContext<AppContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppContext>(name: "database", tags: ["external"])
    .AddCheck<SmtpHealthCheck>(name: "email", tags: ["external"])
    .AddCheck<TwilioHealthCheck>(name: "sms", tags: ["external"]);

builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddTransient<NotifyUsersWithLessThanTenAnswers>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppContext>();

    var adminId = UserSeeder.Execute(context);
    QuestionSeeder.Execute(context, adminId);
    FaqSeeder.Execute(context, adminId);
    AudiovisualResourceSeeder.Execute(context, adminId);
}

app.UseErrorHandler(app.Environment);

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = []  // public access
    });
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var jsonOptions = new JsonWriterOptions { Indented = true };

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        using var stream = new MemoryStream();
        await using (var writer = new Utf8JsonWriter(stream, jsonOptions))
        {
            writer.WriteStartObject();
            writer.WriteString("status", report.Status.ToString());
            writer.WriteString("duration", report.TotalDuration.ToString());
            writer.WriteStartObject("checks");
            foreach (var entry in report.Entries)
            {
                writer.WriteStartObject(entry.Key);
                writer.WriteString("status", entry.Value.Status.ToString());
                writer.WriteString("duration", entry.Value.Duration.ToString());
                if (entry.Value.Description != null)
                    writer.WriteString("description", entry.Value.Description);
                if (entry.Value.Exception != null)
                    writer.WriteString("error", entry.Value.Exception.Message);
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
            writer.WriteEndObject();
        }
        await context.Response.WriteAsync(Encoding.UTF8.GetString(stream.ToArray()));
    }
});

app.MapHealthChecks("/health/self", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => !check.Tags.Contains("external"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        using var stream = new MemoryStream();
        await using (var writer = new Utf8JsonWriter(stream, jsonOptions))
        {
            writer.WriteStartObject();
            writer.WriteString("status", report.Status.ToString());
            writer.WriteString("duration", report.TotalDuration.ToString());
            writer.WriteEndObject();
        }
        await context.Response.WriteAsync(Encoding.UTF8.GetString(stream.ToArray()));
    }
});

var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
recurringJobManager.AddOrUpdate<NotifyUsersWithLessThanTenAnswers>(
    nameof(NotifyUsersWithLessThanTenAnswers),
    x => x.Run(),
    Cron.Daily);

app.Run();