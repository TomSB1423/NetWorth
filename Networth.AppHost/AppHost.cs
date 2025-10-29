using Aspire.Hosting;
using Aspire.Hosting.Azure;
using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// Use a consistent password from configuration or default
string postgresPassword = builder.Configuration["Parameters:postgres-password"] ?? "postgres_dev_password";
IResourceBuilder<ParameterResource> postgresPasswordParam = builder.AddParameter("postgres-password", secret: true);

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("postgres", password: postgresPasswordParam)
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

IResourceBuilder<PostgresDatabaseResource> postgresdb = postgres
    .AddDatabase("networth-db");

IResourceBuilder<AzureFunctionsProjectResource> functions = builder
    .AddAzureFunctionsProject<Networth_Backend_Functions>("functions")
    .WithExternalHttpEndpoints()
    .WithReference(postgresdb);

builder.AddNpmApp("react", "../Networth.Frontend/networth-frontend-react")
    .WithReference(functions)
    .WaitFor(functions)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
