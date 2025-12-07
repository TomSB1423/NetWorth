using Aspire.Hosting.Azure;
using Microsoft.Extensions.Hosting;
using Networth.ServiceDefaults;
using Projects;
using Scalar.Aspire;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> postgresPassword = builder.AddParameter("postgres-password", secret: true);
IResourceBuilder<ParameterResource> gocardlessSecretId = builder.AddParameter("gocardless-secret-id", secret: true);
IResourceBuilder<ParameterResource> gocardlessSecretKey = builder.AddParameter("gocardless-secret-key", secret: true);

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres(ResourceNames.Postgres)
    .WithPassword(postgresPassword)
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

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
    .WaitFor(postgresdb)
    .WithReference(queues)
    .WaitFor(queues)
    .WithEnvironment("Gocardless__SecretId", gocardlessSecretId)
    .WithEnvironment("Gocardless__SecretKey", gocardlessSecretKey)
    .WithHttpHealthCheck("/api/health");

var frontend = builder.AddNpmApp(ResourceNames.React, "../Networth.Frontend", "dev")
    .WithReference(functions)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithEnvironment("VITE_API_URL", functions.GetEndpoint("http"))
    .WithHttpEndpoint(env: "PORT", port: 3000)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

functions.WithEnvironment("Frontend__Url", frontend.GetEndpoint("http"));

// Add Docusaurus documentation site
var docs = builder.AddNpmApp(ResourceNames.Docs, "../Networth.Docs")
    .WithHttpEndpoint(env: "PORT", port: 3001)
    .WithExternalHttpEndpoints()
    .WithEnvironment("BROWSER", "none")
    .WithEnvironment("API_SPEC_URL", ReferenceExpression.Create($"{functions.GetEndpoint("http")}/api/swagger.json"))
    .WithReference(functions)
    .WaitFor(functions)
    .WithExplicitStart();

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

await builder.Build().RunAsync();
