using Microsoft.Extensions.Hosting;
using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);
var postgres = builder.AddPostgres("Postgres");
var postgresdb = postgres.AddDatabase("Postgresdb");

var functions = builder
    .AddAzureFunctionsProject<Projects.Networth_Backend_Functions>("NetworthBackendFunctions")
    .WithExternalHttpEndpoints()
    .WithReference(postgresdb);

var frontend = builder.AddNpmApp("NetworthFrontendReact", "../Networth.Frontend/Networth.Frontend.React")
    .WithReference(functions)
    .WaitFor(functions)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

if (!builder.Environment.IsDevelopment())
{
    var insights = builder.AddAzureApplicationInsights("ApplicationInsights");
    functions.WithReference(insights);
    frontend.WithReference(insights);
}

var scalar = builder.AddScalarApiReference(options =>
{
    options.WithTheme(ScalarTheme.Purple)
        .WithDarkMode();
});

scalar.WithApiReference(functions);


builder.Build().Run();
