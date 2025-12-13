using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
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

            HttpRequestData? request = await context.GetHttpRequestDataAsync();

            if (request == null)
            {
                logger.LogError("Unable to get HttpRequestData from context");
                throw;
            }

            HttpResponseData response = request.CreateResponse();
            var errors = validationEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            response.StatusCode = HttpStatusCode.BadRequest;
            string responseBody = JsonSerializer.Serialize(
                new
                {
                    message = "Validation failed",
                    errors,
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });

            await response.WriteStringAsync(responseBody);
            context.GetInvocationResult().Value = response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            HttpRequestData? request = await context.GetHttpRequestDataAsync();

            if (request == null)
            {
                logger.LogError("Unable to get HttpRequestData from context");
                throw;
            }

            HttpResponseData response = request.CreateResponse();
            response.StatusCode = HttpStatusCode.InternalServerError;

            string responseBody;
            if (environment.IsDevelopment())
            {
                responseBody = JsonSerializer.Serialize(
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
                responseBody = JsonSerializer.Serialize(
                    new
                    {
                        message = "An internal server error occurred. Please try again later.",
                    },
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    });
            }

            await response.WriteStringAsync(responseBody);
            context.GetInvocationResult().Value = response;
        }
    }
}
