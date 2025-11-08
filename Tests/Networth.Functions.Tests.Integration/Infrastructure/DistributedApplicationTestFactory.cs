namespace Networth.Functions.Tests.Integration.Infrastructure;

using System.Reflection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

internal static class DistributedApplicationTestFactory
{
    /// <summary>
    ///     Creates an <see cref="IDistributedApplicationTestingBuilder" /> for the specified app host assembly.
    /// </summary>
    /// <returns>
    ///     <placeholder>A <see cref="Task" /> representing the asynchronous operation.</placeholder>
    /// </returns>
    public static async Task<IDistributedApplicationTestingBuilder> CreateAsync(string appHostAssemblyPath, ITestOutputHelper? testOutput)
    {
        var appHostProjectName = Path.GetFileNameWithoutExtension(appHostAssemblyPath) ??
                                 throw new InvalidOperationException("AppHost assembly was not found.");

        var appHostAssembly = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, appHostAssemblyPath));

        var appHostType = appHostAssembly.GetTypes().FirstOrDefault(t => t.Name.EndsWith("_AppHost"))
                          ?? throw new InvalidOperationException("Generated AppHost type not found.");

        var builder = await DistributedApplicationTestingBuilder.CreateAsync(appHostType);

        builder.WithRandomVolumeNames();
        builder.WithContainersLifetime(ContainerLifetime.Session);

        builder.Services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSimpleConsole();
            logging.AddFakeLogging();
            if (testOutput is not null)
            {
                logging.AddXUnit(testOutput);
            }

            logging.SetMinimumLevel(LogLevel.Trace);
            logging.AddFilter("Aspire", LogLevel.Trace);
            logging.AddFilter(builder.Environment.ApplicationName, LogLevel.Trace);
        });

        return builder;
    }
}
