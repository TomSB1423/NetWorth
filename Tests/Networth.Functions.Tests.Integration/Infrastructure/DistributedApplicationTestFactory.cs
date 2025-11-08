using Microsoft.Extensions.Logging;
using Projects;
using Xunit.Abstractions;

namespace Networth.Functions.Tests.Integration.Infrastructure;

internal static class DistributedApplicationTestFactory
{
    /// <summary>
    ///     Creates an <see cref="IDistributedApplicationTestingBuilder" /> for the specified app host assembly.
    /// </summary>
    public static async Task<IDistributedApplicationTestingBuilder>
        CreateAsync(ITestOutputHelper? testOutput, bool enableDashboard = false)
    {
        IDistributedApplicationTestingBuilder? builder =
            await DistributedApplicationTestingBuilder.CreateAsync<Networth_AppHost>(
                [],
                (appOptions, hostSettings) =>
                {
                    appOptions.DisableDashboard = !enableDashboard;
                    appOptions.AllowUnsecuredTransport = enableDashboard;
                });

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
