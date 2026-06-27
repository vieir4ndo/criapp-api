using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using CRIApp.Api.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using AppContext = CRIApp.Repository.AppContext;

namespace CRIApp.Api.Extensions;

[ExcludeFromCodeCoverage]
public static class HealthChecksExtensions
{
    public static IHealthChecksBuilder AddAppHealthChecks(this IServiceCollection services)
    {
        return services.AddHealthChecks()
            .AddDbContextCheck<AppContext>(name: "database", tags: ["external"])
            .AddCheck<MailjetHealthCheck>(name: "email", tags: ["external"])
            .AddCheck<TwilioHealthCheck>(name: "sms", tags: ["external"]);
    }

    public static WebApplication UseHealthCheckEndpoints(this WebApplication app)
    {
        var jsonOptions = new JsonWriterOptions { Indented = true };

        app.MapHealthChecks("/health", new HealthCheckOptions
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

        app.MapHealthChecks("/health/self", new HealthCheckOptions
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

        return app;
    }
}
