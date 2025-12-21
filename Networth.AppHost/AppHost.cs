using Aspire.Hosting.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Networth.ServiceDefaults;
using Projects;
using Scalar.Aspire;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<ParameterResource> postgresPassword = builder.AddParameter("postgres-password", secret: true);
IResourceBuilder<ParameterResource> gocardlessSecretId = builder.AddParameter("gocardless-secret-id", secret: true);
IResourceBuilder<ParameterResource> gocardlessSecretKey = builder.AddParameter("gocardless-secret-key", secret: true);

// Firebase authentication configuration
IResourceBuilder<ParameterResource> firebaseApiKey = builder.AddParameter("firebase-api-key");
IResourceBuilder<ParameterResource> firebaseAuthDomain = builder.AddParameter("firebase-auth-domain");
IResourceBuilder<ParameterResource> firebaseProjectId = builder.AddParameter("firebase-project-id");

var postgres = builder
    .AddPostgres(ResourceNames.Postgres)
    .WithPassword(postgresPassword)
    .WithDataVolume("networth-postgres-data");

var postgresdb = postgres
    .AddDatabase(ResourceNames.NetworthDb);

// Add Azure Storage for queues
IResourceBuilder<AzureStorageResource> storage = builder
    .AddAzureStorage(ResourceNames.Storage)
    .RunAsEmulator();

IResourceBuilder<AzureBlobStorageResource> blobs = storage.AddBlobs("blobs");
IResourceBuilder<AzureTableStorageResource> tables = storage.AddTables("tables");
IResourceBuilder<AzureQueueStorageResource> queues = storage.AddQueues(ResourceNames.Queues);

storage.AddQueue(ResourceNames.AccountSyncQueue);
storage.AddQueue(ResourceNames.InstitutionSyncQueue);
storage.AddQueue(ResourceNames.CalculateRunningBalanceQueue);

IResourceBuilder<AzureFunctionsProjectResource> functions = builder
    .AddAzureFunctionsProject<Networth_Functions>(ResourceNames.Functions)
    .WithExternalHttpEndpoints()
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithReference(queues)
    .WithReference(blobs)
    .WithReference(tables)
    .WaitFor(queues)
    .WithEnvironment("AzureWebJobsStorage", blobs.Resource.ConnectionStringExpression)
    .WithHttpHealthCheck("/api/health");

// Configure GoCardless and Firebase
functions
    .WithEnvironment("Gocardless__SecretId", gocardlessSecretId)
    .WithEnvironment("Gocardless__SecretKey", gocardlessSecretKey)
    .WithEnvironment("Firebase__ProjectId", firebaseProjectId);

var frontend = builder.AddNpmApp(ResourceNames.React, "../Networth.Frontend", "dev")
    .WithReference(functions)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithEnvironment("VITE_API_URL", functions.GetEndpoint("http"))
    .WithEnvironment("VITE_FIREBASE_API_KEY", firebaseApiKey)
    .WithEnvironment("VITE_FIREBASE_AUTH_DOMAIN", firebaseAuthDomain)
    .WithEnvironment("VITE_FIREBASE_PROJECT_ID", firebaseProjectId)
    .WithHttpEndpoint(env: "PORT", port: 3000)
    .WithExternalHttpEndpoints();

functions.WithEnvironment("Frontend__Url", frontend.GetEndpoint("http"));

// Add Docusaurus documentation site
var docs = builder.AddNpmApp(ResourceNames.Docs, "../Networth.Docs")
    .WithHttpEndpoint(env: "PORT", port: 3001)
    .WithExternalHttpEndpoints()
    .WithEnvironment("BROWSER", "none")
    .WithEnvironment("API_SPEC_URL", ReferenceExpression.Create($"{functions.GetEndpoint("http")}/api/swagger.json"))
    .WithReference(functions)
    .WaitFor(functions)
    .WithExplicitStart()
    .ExcludeFromManifest(); // Local dev only

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
}).ExcludeFromManifest(); // Local dev only


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
