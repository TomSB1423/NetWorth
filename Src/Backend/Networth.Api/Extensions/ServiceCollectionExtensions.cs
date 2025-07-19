using Microsoft.AspNetCore.HttpLogging;
using Networth.Interfaces;
using Networth.Options;
using Serilog;

namespace Networth.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOptionsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsWithValidateOnStart<NetworthOptions>()
            .BindConfiguration("Networth")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptionsWithValidateOnStart<GocardlessOptions>()
            .BindConfiguration("Gocardless")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public static IHostBuilder AddSerilogLogging(this IHostBuilder host)
    {
        return host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services));
    }

    public static IServiceCollection AddHttpRequestLogging(this IServiceCollection services)
    {
        return services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
            logging.ResponseHeaders.Add("Content-Type");
            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;
        });
    }

    public static WebApplication UseLoggingMiddleware(this WebApplication app)
    {
        app.UseHttpLogging();
        app.UseSerilogRequestLogging();

        return app;
    }

    public static WebApplication RegisterFeatureRouters(this WebApplication app)
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var routerTypes = assembly.GetTypes()
            .Where(t => !t.IsInterface && !t.IsAbstract && t.IsClass && typeof(IFeatureRouter).IsAssignableFrom(t))
            .ToArray();

        foreach (var routerType in routerTypes)
        {
            if (Activator.CreateInstance(routerType) is IFeatureRouter router)
            {
                router.RegisterRoutes(app);
            }
        }

        return app;
    }
}
