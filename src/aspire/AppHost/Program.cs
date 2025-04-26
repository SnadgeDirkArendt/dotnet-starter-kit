using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel; // Für WithPort
using Aspire.Hosting.Postgres;

var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("pg-username", "pgadmin");
var password = builder.AddParameter("pg-password", "pgadmin");


// PostgreSQL Container
var postgres = builder.AddPostgres("db", username, password, 5432)
    .WithImage("postgres:15-alpine")
    .WithDataVolume()
    .AddDatabase("fullstackhero");

// WebAPI
var webApi = builder.AddProject<Projects.Server>("webapi")
        .WaitFor(postgres)
        .WithEnvironment("ConnectionStrings__DefaultConnection", "Host=localhost;Port=5432;Database=fullstackhero;Username=pgadmin;Password=pgadmin");

// Blazor Frontend
var blazor = builder.AddProject<Projects.Client>("blazor")
    .WithReference(webApi);

// Monitoring - Grafana
var grafana = builder.AddContainer("grafana", "grafana/grafana:latest")
    .WithHttpEndpoint(port: 3000, targetPort: 3000, name: "http");

builder.Build().Run();
