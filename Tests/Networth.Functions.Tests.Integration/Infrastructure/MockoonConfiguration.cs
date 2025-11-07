namespace Networth.Backend.Functions.Tests.Integration.Infrastructure;

/// <summary>
///     Configuration for Mockoon test containers.
/// </summary>
public static class MockoonConfiguration
{
    /// <summary>
    ///     Gets the Mockoon CLI Docker image.
    /// </summary>
    public const string Image = "mockoon/cli:latest";

    /// <summary>
    ///     Gets the Mockoon container port.
    /// </summary>
    public const int Port = 3000;

    /// <summary>
    ///     Gets the path to the GoCardless mock data file.
    /// </summary>
    public const string GoCardlessDataFile = "mockoon-gocardless-spec.json";

    /// <summary>
    ///     Gets the container mount path for mock data.
    /// </summary>
    public const string ContainerDataPath = "/data/mockoon-gocardless-spec.json";

    /// <summary>
    ///     Gets the initialization delay in milliseconds.
    /// </summary>
    public const int InitializationDelayMs = 2000;
}
