namespace Networth.Functions.Tests.Integration;

/// <summary>
///     Defines the integration test collection.
///     Tests in this collection run sequentially to avoid port conflicts
///     from concurrent Aspire application instances (e.g., frontend on port 3000).
/// </summary>
[CollectionDefinition("Integration")]
public class IntegrationTestCollection;
