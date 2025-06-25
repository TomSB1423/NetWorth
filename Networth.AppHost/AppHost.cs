using Scalar.Aspire;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

var functions = builder
    .AddAzureFunctionsProject<Projects.Networth_Backend_Functions>("functions")
    .WithExternalHttpEndpoints()
    .WithReference(postgresdb);

builder.AddNpmApp("react", "../Networth.Frontend/Networth.Frontend.React")
    .WithReference(functions)
    .WaitFor(functions)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

var scalar = builder.AddScalarApiReference(options =>
{
    options.WithTheme(ScalarTheme.Purple)
        .WithDarkMode();
});

scalar.WithApiReference(functions);


builder.Build().Run();
