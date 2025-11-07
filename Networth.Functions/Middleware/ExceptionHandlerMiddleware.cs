using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;

namespace Networth.Functions.Middleware;

/// <summary>
///     Responsible for handling exceptions that occur during the execution of Azure Functions.
/// </summary>
public class ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger) : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            HttpRequestData? request = await context.GetHttpRequestDataAsync();
            HttpResponseData response = request!.CreateResponse();

            var (statusCode, errorResponse) = ex switch
            {
                ValidationException validationEx => HandleValidationException(validationEx),
                _ => HandleInternalServerError(ex)
            };

            response.StatusCode = statusCode;
            string responseBody = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteStringAsync(responseBody);
            context.GetInvocationResult().Value = response;
        }
    }

    private (HttpStatusCode StatusCode, object ErrorResponse) HandleValidationException(ValidationException ex)
    {
        var errors = ex.Errors.Select(e => e.ErrorMessage).ToArray();
        logger.LogWarning("Validation failed: {ValidationErrors}", string.Join(", ", errors));
        return (HttpStatusCode.BadRequest, new { errors });
    }

    private (HttpStatusCode StatusCode, object ErrorResponse) HandleInternalServerError(Exception ex)
    {
        logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
        return (HttpStatusCode.InternalServerError, new
        {
            errors = new[] { "An internal server error occurred. Please try again later." }
        });
    }
}
