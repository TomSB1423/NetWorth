using Networth.Functions.Tests.Integration.Fixtures;

namespace Networth.Functions.Tests.Integration;

/// <summary>
///     Defines the integration test collection.
///     Tests in this collection run sequentially to avoid port conflicts
///     from concurrent Aspire application instances.
///     The MockoonTestFixture is shared across all test classes in the collection
///     to avoid creating/destroying Docker containers between test classes.
/// </summary>
[CollectionDefinition("Integration")]
public class IntegrationTestCollection : ICollectionFixture<MockoonTestFixture>;
