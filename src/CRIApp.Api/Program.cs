using CRIApp.Api.Extensions;
using CRIApp.Api.Middlewares;
using CRIApp.Application.Extensions;
using CRIApp.Repository.Seeders;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using AppContext = CRIApp.Repository.AppContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddSettingsConfiguration(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddHangfireServices(builder.Configuration);
builder.Services.AddFluentValidationLocalization();
builder.Services.AddLocalization();
builder.Services.AddJobServices();

builder.Services.AddDbContext<AppContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddAppHealthChecks();

builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddControllers();

builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppContext>();

    var adminId = UserSeeder.Execute(context);
    QuestionAdviceSeeder.Execute(context, adminId);
    FaqSeeder.Execute(context, adminId);
    AudiovisualResourceSeeder.Execute(context, adminId);
    AwardSeeder.Execute(context);
}

app.UseErrorHandler(app.Environment);

app.UseRequestLocalization(options =>
{
    options.SetDefaultCulture("es");
    options.AddSupportedCultures("es", "en", "pt");
    options.AddSupportedUICultures("es", "en", "pt");
});

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = []
    });
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHealthCheckEndpoints();

app.UseRecurringJobs();

app.Run();