using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Networth.Functions.Middleware;

/// <summary>
///     Responsible for handling exceptions that occur during the execution of Azure Functions.
/// </summary>
public class ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, IHostEnvironment environment)
    : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException validationEx)
        {
            logger.LogWarning(
                "Validation failed: {ValidationErrors}",
                string.Join(", ", validationEx.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")));

            var httpContext = context.GetHttpContext();

            if (httpContext == null)
            {
                logger.LogError("Unable to get HttpContext from context");
                throw;
            }

            var errors = validationEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(
                new
                {
                    message = "Validation failed",
                    errors,
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

            var httpContext = context.GetHttpContext();

            if (httpContext == null)
            {
                logger.LogError("Unable to get HttpContext from context");
                throw;
            }

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/json";

            if (environment.IsDevelopment())
            {
                await httpContext.Response.WriteAsJsonAsync(
                    new
                    {
                        message = ex.Message,
                        type = ex.GetType().Name,
                        stackTrace = ex.StackTrace,
                        innerException = ex.InnerException?.Message,
                    },
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    });
            }
            else
            {
                await httpContext.Response.WriteAsJsonAsync(
                    new
                    {
                        message = "An internal server error occurred. Please try again later.",
                    },
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    });
            }
        }
    }
}
