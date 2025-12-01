using Aspire.Hosting.Azure;
using Microsoft.Extensions.Hosting;
using Networth.ServiceDefaults;
using Projects;
using Scalar.Aspire;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres(ResourceNames.Postgres)
    .WithDataVolume();

IResourceBuilder<PostgresDatabaseResource> postgresdb = postgres
    .AddDatabase(ResourceNames.NetworthDb);

// Add Azure Storage for queues
IResourceBuilder<AzureStorageResource> storage = builder
    .AddAzureStorage(ResourceNames.Storage)
    .RunAsEmulator();

IResourceBuilder<AzureQueueStorageResource> queues = storage.AddQueues(ResourceNames.Queues);

IResourceBuilder<AzureFunctionsProjectResource> functions = builder
    .AddAzureFunctionsProject<Networth_Functions>(ResourceNames.Functions)
    .WithExternalHttpEndpoints()
    .WithReference(postgresdb)
    .WithReference(queues);

builder.AddNpmApp(ResourceNames.React, "../Networth.Frontend")
    .WithReference(functions)
    .WaitFor(functions)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithEnvironment("REACT_APP_BACKEND_URL", functions.GetEndpoint("http"))
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

// Add Scalar API Reference
IResourceBuilder<ScalarResource> scalar = builder.AddScalarApiReference("api-reference", options =>
{
    options
    .WithTheme(ScalarTheme.Purple)
    .ExpandAllTags()
    .ExpandAllResponses()
    .HideClientButton()
    .HideDarkModeToggle()
        .AddMetadata("title", "Networth API Reference")
        .AddMetadata("description", "Unified API documentation for Networth backend services");
});

// Register Functions service with Scalar
scalar.WithApiReference(functions, options =>
{
    options
        .AddDocument("v1", "Networth API")
        .WithOpenApiRoutePattern("/api/swagger.json")
        .AddServer("/api", "Azure Functions API")
        .AddMetadata("summary", "Public endpoints exposed by the Networth Azure Functions app");
});

if (builder.Environment.IsDevelopment())
{
    postgres.WithPgAdmin(pgAdmin =>
    {
        pgAdmin
            .WithHostPort(5050)
            .WithExplicitStart();
    });
}

builder.Build().Run();
