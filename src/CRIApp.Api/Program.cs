using CRIApp.Api.Middlewares;
using CRIApp.Application.Extensions;
using CRIApp.Application.Jobs;
using CRIApp.Domain.Settings;
using CRIApp.Repository.Seeders;
using FluentValidation;
using Hangfire;
using Hangfire.LiteDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AppContext = CRIApp.Repository.AppContext;

var builder = WebApplication.CreateBuilder(args);

// TODO: Isolate in extensions
var jwtSettingsConfigurationSection = builder.Configuration.GetSection(JwtTokenSettings.SectionName);
builder.Services.Configure<JwtTokenSettings>(jwtSettingsConfigurationSection);
var connectionStrings = builder.Configuration.GetSection(ConnectionStrings.SectionName).Get<ConnectionStrings>();
var jwtSettings = jwtSettingsConfigurationSection.Get<JwtTokenSettings>();
var twilioSettings = builder.Configuration.GetSection(TwilioSettings.SectionName);
builder.Services.Configure<TwilioSettings>(twilioSettings);
var mailjetSettings = builder.Configuration.GetSection(MailjetSettings.SectionName);
builder.Services.Configure<MailjetSettings>(mailjetSettings);
// HF
builder.Services.AddHangfire(configuration =>
{
    configuration.UseLiteDbStorage("./hf.db");
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
builder.Services.AddDbContext<AppContext>();

using (var context = new AppContext())
{
    context.Database.EnsureCreated(); // Certificar-se de que o banco de dados existe

    var adminId = UserSeeder.Execute(context);
    QuestionSeeder.Execute(context, adminId);
    FaqSeeder.Execute(context, adminId);
    AudiovisualResourceSeeder.Execute(context, adminId);
}

builder.Services.AddServices();
builder.Services.AddRepositories();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseErrorHandler();

app.UseHangfireDashboard("/hangfire");
app.MapHangfireDashboard();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var recurringJobManager = app.Services.GetService<IRecurringJobManager>();
recurringJobManager.AddOrUpdate(
    nameof(NotifyUsersWithLessThanTenAnswers),
    () => new NotifyUsersWithLessThanTenAnswers().Run(),
    Cron.Minutely);

app.Run();