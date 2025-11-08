using Aspire.Hosting.Azure;
using Microsoft.Extensions.Hosting;
using MyApp.AppHost;
using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres(ResourceNames.Postgres)
    .WithPgAdmin(pgAdmin => pgAdmin.WithHostPort(5050));

if (!builder.Environment.IsEnvironment("Test"))
{
    postgres.WithDataVolume();
}

IResourceBuilder<PostgresDatabaseResource> postgresdb = postgres
    .AddDatabase(ResourceNames.NetworthDb);

IResourceBuilder<AzureFunctionsProjectResource> functions = builder
    .AddAzureFunctionsProject<Networth_Functions>(ResourceNames.Functions)
    .WithExternalHttpEndpoints()
    .WithReference(postgresdb);

builder.AddNpmApp(ResourceNames.React, "../Networth.Frontend")
    .WithReference(functions)
    .WaitFor(functions)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
