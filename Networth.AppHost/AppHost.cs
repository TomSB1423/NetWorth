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

IResourceBuilder<ParameterResource> entraClientId = builder.AddParameter("entra-client-id");
IResourceBuilder<ParameterResource> entraTenantId = builder.AddParameter("entra-tenant-id");
IResourceBuilder<ParameterResource> entraApiClientId = builder.AddParameter("entra-api-client-id");
IResourceBuilder<ParameterResource> entraInstance = builder.AddParameter("entra-instance");

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

// Configure GoCardless and Azure AD
functions
    .WithEnvironment("Gocardless__SecretId", gocardlessSecretId)
    .WithEnvironment("Gocardless__SecretKey", gocardlessSecretKey)
    .WithEnvironment("AzureAd__Instance", entraInstance)
    .WithEnvironment("AzureAd__TenantId", entraTenantId)
    .WithEnvironment("AzureAd__ClientId", entraApiClientId)
    .WithEnvironment("AzureAd__Audience", entraApiClientId); // In CIAM, Audience = API Client ID

var frontend = builder.AddNpmApp(ResourceNames.React, "../Networth.Frontend", "dev")
    .WithReference(functions)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithEnvironment("VITE_API_URL", functions.GetEndpoint("http"))
    .WithEnvironment("VITE_ENTRA_CLIENT_ID", entraClientId)
    .WithEnvironment("VITE_ENTRA_TENANT_ID", entraTenantId)
    .WithEnvironment("VITE_ENTRA_API_CLIENT_ID", entraApiClientId)
    .WithEnvironment("VITE_ENTRA_INSTANCE", entraInstance)
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
