using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace Networth.Backend.Functions.Middleware;

/// <summary>
///     Responsible for handling exceptions that occur during the execution of Azure Functions.
/// </summary>
public class ExceptionHandlerMiddleware : IFunctionsWorkerMiddleware
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
            response.StatusCode = HttpStatusCode.InternalServerError;

            var errorMessage = new
            {
                Message = "An unhandled exception occurred. Please try again later", Exception = ex.Message,
            };
            string responseBody = JsonSerializer.Serialize(errorMessage);
            await response.WriteStringAsync(responseBody);
            context.GetInvocationResult().Value = response;
        }
    }
}
