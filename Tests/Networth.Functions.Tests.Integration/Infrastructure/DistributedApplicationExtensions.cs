using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace Networth.Functions.Tests.Integration.Infrastructure;

/// <summary>
///     Extensions for working with distributed applications in tests.
/// </summary>
public static class DistributedApplicationExtensions
{
    /// <summary>
    ///     Sets the container lifetime for all container resources in the application.
    /// </summary>
    /// <typeparam name="TBuilder">The type of the distributed application testing builder.</typeparam>
    public static TBuilder WithContainersLifetime<TBuilder>(this TBuilder builder, ContainerLifetime containerLifetime)
        where TBuilder : IDistributedApplicationTestingBuilder
    {
        var containerLifetimeAnnotations = builder.Resources.SelectMany(r => r.Annotations
                .OfType<ContainerLifetimeAnnotation>()
                .Where(c => c.Lifetime != containerLifetime))
            .ToList();

        foreach (var annotation in containerLifetimeAnnotations)
        {
            annotation.Lifetime = containerLifetime;
        }

        return builder;
    }

    /// <summary>
    ///     Replaces all named volumes with anonymous volumes so they're isolated across test runs and from the volume the app
    ///     uses during development.
    /// </summary>
    /// <remarks>
    ///     Note that if multiple resources share a volume, the volume will instead be given a random name so that it's still
    ///     shared across those resources in the test run.
    /// </remarks>
    /// <typeparam name="TBuilder">The type of the distributed application testing builder.</typeparam>
    public static TBuilder WithRandomVolumeNames<TBuilder>(this TBuilder builder)
        where TBuilder : IDistributedApplicationTestingBuilder
    {
        // Named volumes that aren't shared across resources should be replaced with anonymous volumes.
        // Named volumes shared by multiple resources need to have their name randomized but kept shared across those resources.

        // Find all shared volumes and make a map of their original name to a new randomized name
        var allResourceNamedVolumes = builder.Resources.SelectMany(r => r.Annotations
                .OfType<ContainerMountAnnotation>()
                .Where(m => m.Type == ContainerMountType.Volume && !string.IsNullOrEmpty(m.Source))
                .Select(m => (Resource: r, Volume: m)))
            .ToList();
        HashSet<string> seenVolumes = new();
        Dictionary<string, string> renamedVolumes = new();
        foreach (var resourceVolume in allResourceNamedVolumes)
        {
            var name = resourceVolume.Volume.Source!;
            if (!seenVolumes.Add(name) && !renamedVolumes.ContainsKey(name))
            {
                renamedVolumes[name] = $"{name}-{Convert.ToHexString(RandomNumberGenerator.GetBytes(4))}";
            }
        }

        // Replace all named volumes with randomly named or anonymous volumes
        foreach (var resourceVolume in allResourceNamedVolumes)
        {
            var resource = resourceVolume.Resource;
            var volume = resourceVolume.Volume;
            var newName = renamedVolumes.TryGetValue(volume.Source!, out var randomName)
                ? randomName
                : null;
            ContainerMountAnnotation newMount = new(newName, volume.Target, ContainerMountType.Volume, volume.IsReadOnly);
            resource.Annotations.Remove(volume);
            resource.Annotations.Add(newMount);
        }

        return builder;
    }

    /// <summary>
    ///     Waits for the specified resource to reach the specified state.
    /// </summary>
    public static Task WaitForResource(
        this DistributedApplication app,
        string resourceName,
        string? targetState = null,
        CancellationToken cancellationToken = default)
    {
        targetState ??= KnownResourceStates.Running;
        var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();

        return resourceNotificationService.WaitForResourceAsync(resourceName, targetState, cancellationToken);
    }

    /// <summary>
    ///     Waits for all resources in the application to reach one of the specified states.
    /// </summary>
    /// <remarks>
    ///     If <paramref name="targetStates" /> is null, the default states are <see cref="KnownResourceStates.Running" /> and
    ///     <see cref="KnownResourceStates.Hidden" />.
    /// </remarks>
    public static async Task WaitForResourcesAsync(
        this DistributedApplication app,
        IEnumerable<string>? targetStates = null,
        CancellationToken cancellationToken = default)
    {
        var logger = app.Services.GetRequiredService<ILoggerFactory>()
            .CreateLogger($"{nameof(Networth)}.{nameof(WaitForResourcesAsync)}");

        var targetStatesList = (targetStates ?? [KnownResourceStates.Running, ..KnownResourceStates.TerminalStates]).ToList();
        var applicationModel = app.Services.GetRequiredService<DistributedApplicationModel>();

        Dictionary<string, Task<(string Name, string State)>> resourceTasks = new();

        foreach (var resource in applicationModel.Resources)
        {
            if (resource is IResourceWithoutLifetime)
            {
                continue;
            }

            resourceTasks[resource.Name] = GetResourceWaitTask(resource.Name, targetStatesList, cancellationToken);
        }

        logger.LogInformation(
            "Waiting for resources [{Resources}] to reach one of target states [{TargetStates}].",
            string.Join(',', resourceTasks.Keys),
            string.Join(',', targetStatesList));

        while (resourceTasks.Count > 0)
        {
            var completedTask = await Task.WhenAny(resourceTasks.Values);
            var (completedResourceName, targetStateReached) = await completedTask;

            if (targetStateReached == KnownResourceStates.FailedToStart)
            {
                throw new DistributedApplicationException($"Resource '{completedResourceName}' failed to start.");
            }

            resourceTasks.Remove(completedResourceName);

            logger.LogInformation(
                "Wait for resource '{ResourceName}' completed with state '{ResourceState}'",
                completedResourceName,
                targetStateReached);

            // Ensure resources being waited on still exist
            var remainingResources = resourceTasks.Keys.ToList();
            for (var i = remainingResources.Count - 1; i >= 0; i--)
            {
                var name = remainingResources[i];
                if (applicationModel.Resources.Any(r => r.Name == name))
                {
                    continue;
                }

                logger.LogInformation("Resource '{ResourceName}' was deleted while waiting for it.", name);
                resourceTasks.Remove(name);
                remainingResources.RemoveAt(i);
            }

            if (resourceTasks.Count > 0)
            {
                logger.LogInformation(
                    "Still waiting for resources [{Resources}] to reach one of target states [{TargetStates}].",
                    string.Join(',', remainingResources),
                    string.Join(',', targetStatesList));
            }
        }

        logger.LogInformation("Wait for all resources completed successfully!");

        async Task<(string Name, string State)> GetResourceWaitTask(
            string resourceName,
            IEnumerable<string> states,
            CancellationToken token)
        {
            var state = await app.ResourceNotifications.WaitForResourceAsync(resourceName, states, token);
            return (resourceName, state);
        }
    }

    /// <summary>
    ///     Gets the app host and resource logs from the application.
    /// </summary>
    /// <returns> A tuple containing the app host logs and resource logs. </returns>
    public static (IReadOnlyList<FakeLogRecord> AppHostLogs, IReadOnlyList<FakeLogRecord> ResourceLogs) GetLogs(this DistributedApplication app)
    {
        var environment = app.Services.GetRequiredService<IHostEnvironment>();
        var logCollector = app.Services.GetFakeLogCollector();
        var logs = logCollector.GetSnapshot();
        var appHostLogs = logs.Where(l => l.Category?.StartsWith($"{environment.ApplicationName}.Resources") == false).ToList();
        var resourceLogs = logs.Where(l => l.Category?.StartsWith($"{environment.ApplicationName}.Resources") == true).ToList();

        return (appHostLogs, resourceLogs);
    }

    /// <summary>
    ///     Creates an <see cref="HttpClient" /> configured to communicate with the specified resource.
    /// </summary>
    public static HttpClient CreateHttpClient(this DistributedApplication app, string resourceName, bool useHttpClientFactory)
        => app.CreateHttpClient(resourceName, null, useHttpClientFactory);

    /// <summary>
    ///     Creates an <see cref="HttpClient" /> configured to communicate with the specified resource.
    /// </summary>
    public static HttpClient CreateHttpClient(
        this DistributedApplication app,
        string resourceName,
        string? endpointName,
        bool useHttpClientFactory)
    {
        if (useHttpClientFactory)
        {
            return app.CreateHttpClient(resourceName, endpointName);
        }

        // Don't use the HttpClientFactory to create the HttpClient so, e.g., no resilience policies are applied
        HttpClient httpClient = new()
        {
            BaseAddress = app.GetEndpoint(resourceName, endpointName),
        };

        return httpClient;
    }

    /// <summary>
    ///     Creates an <see cref="HttpClient" /> configured to communicate with the specified resource with custom
    ///     configuration.
    /// </summary>
    public static HttpClient CreateHttpClient(
        this DistributedApplication app,
        string resourceName,
        string? endpointName,
        Action<IHttpClientBuilder> configure)
    {
        var services = new ServiceCollection()
            .AddHttpClient()
            .ConfigureHttpClientDefaults(configure)
            .BuildServiceProvider();
        var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();

        var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = app.GetEndpoint(resourceName, endpointName);

        return httpClient;
    }
}
