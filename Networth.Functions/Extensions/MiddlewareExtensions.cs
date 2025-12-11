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
        var authOptions = builder.Configuration.GetSection(AuthenticationOptions.SectionName).Get<AuthenticationOptions>();
        if (authOptions == null)
        {
            throw new InvalidOperationException($"Failed to retrieve '{AuthenticationOptions.SectionName}' section from configuration.");
        }

        builder.UseMiddleware<ExceptionHandlerMiddleware>();

        if (authOptions.UseMockAuthentication)
        {
            builder.UseMiddleware<MockAuthenticationMiddleware>();
        }
        else
        {
            builder.UseMiddleware<JwtBearerAuthenticationMiddleware>();
        }

        return builder;
    }
}
