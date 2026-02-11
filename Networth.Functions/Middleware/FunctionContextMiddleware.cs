using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Networth.Functions.Authentication;

namespace Networth.Functions.Middleware;

/// <summary>
///     Middleware that captures the FunctionContext and makes it available via IFunctionContextAccessor.
///     This must run before any other middleware that needs to access FunctionContext from services.
/// </summary>
public class FunctionContextMiddleware(IFunctionContextAccessor functionContextAccessor) : IFunctionsWorkerMiddleware
{
    /// <inheritdoc />
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        functionContextAccessor.FunctionContext = context;
        try
        {
            await next(context);
        }
        finally
        {
            functionContextAccessor.FunctionContext = null;
        }
    }
}
