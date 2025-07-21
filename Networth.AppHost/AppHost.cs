using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);
var insights = builder.AddAzureApplicationInsights("MyApplicationInsights");
var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

var functions = builder
    .AddAzureFunctionsProject<Projects.Networth_Backend_Functions>("functions")
    .WithExternalHttpEndpoints()
    .WithReference(postgresdb)
    .WithReference(insights);

builder.AddNpmApp("react", "../Networth.Frontend/Networth.Frontend.React")
    .WithReference(functions)
    .WaitFor(functions)
    .WithEnvironment("BACKEND_URL", $"https+http://_dashboard.{functions.Resource.Name}")
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile()
    .WithReference(insights);


var scalar = builder.AddScalarApiReference(options =>
{
    options.WithTheme(ScalarTheme.Purple)
        .WithDarkMode();
});

scalar.WithApiReference(functions);


builder.Build().Run();
