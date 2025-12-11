using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Networth.Functions.Authentication;
using Networth.Functions.Middleware;

namespace Networth.Functions.Extensions;

public static class MiddlewareExtensions
{
    public static FunctionsApplicationBuilder ConfigureMiddleware(this FunctionsApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceptionHandlerMiddleware>();
        builder.UseMiddleware<AuthenticationMiddleware>();

        return builder;
    }
}
