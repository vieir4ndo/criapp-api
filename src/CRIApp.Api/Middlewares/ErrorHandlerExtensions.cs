using System.Diagnostics.CodeAnalysis;
using System.Net;
using CRIApp.Api.Controllers.Responses;
using CRIApp.Application.Resources;
using CRIApp.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CRIApp.Api.Middlewares;

public static class ErrorHandlerExtensions
{
    public static void UseErrorHandler(this IApplicationBuilder appBuilder, IWebHostEnvironment  env)
    {
        var logger = appBuilder.ApplicationServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("CRIApp.Api.Middlewares.ErrorHandler");

        appBuilder.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                var localizer = context.RequestServices.GetRequiredService<IStringLocalizer<AppMessages>>();

                var exceptionHandlerFeature = context
                    .Features
                    .Get<IExceptionHandlerFeature>();
                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();

                if (exceptionHandlerFeature != null)
                {
                    switch (exceptionHandlerPathFeature?.Error)
                    {
                        case NotFoundException notFoundException:
                        {
                            var response = new ApiResponse<object>(false , messages: notFoundException.Errors);

                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                            context.Response.ContentType = "application/json";

                            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                            break;
                        }
                        case ValidationException validationException:
                        {
                            var response = new ApiResponse<object>(false, messages: validationException.Errors);

                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            context.Response.ContentType = "application/json";

                            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                            break;
                        }
                        default:
                        {
                            var error = exceptionHandlerPathFeature?.Error;
                            logger.LogError(error,
                                "Unhandled exception on {Method} {Path}",
                                context.Request.Method,
                                context.Request.Path);

                            ApiResponse<object> response;
                            if (env.IsDevelopment())
                            {
                                response = new ApiResponse<object>(exceptionHandlerFeature.Error.Message);
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                context.Response.ContentType = "application/json";
                            }
                            else
                            {
                                response = new ApiResponse<object>(localizer["GenericError"]);
                            }

                            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                            break;
                        }
                    }
                }
            });
        });
    }
}