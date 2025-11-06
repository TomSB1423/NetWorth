using Aspire.Hosting;
using Aspire.Hosting.Azure;
using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// Add Azure Storage for Functions runtime (required for isolated worker)
IResourceBuilder<AzureStorageResource> storage = builder
    .AddAzureStorage("storage")
    .RunAsEmulator();

IResourceBuilder<AzureBlobStorageResource> blobs = storage.AddBlobs("blobs");

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("postgres")
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

IResourceBuilder<PostgresDatabaseResource> postgresdb = postgres
    .AddDatabase("networth-db");

IResourceBuilder<AzureFunctionsProjectResource> functions = builder
    .AddAzureFunctionsProject<Networth_Backend_Functions>("functions")
    .WithExternalHttpEndpoints()
    .WithReference(blobs)
    .WithReference(postgresdb);

builder.AddNpmApp("react", "../Networth.Frontend/networth-frontend-react")
    .WithReference(functions)
    .WaitFor(functions)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
