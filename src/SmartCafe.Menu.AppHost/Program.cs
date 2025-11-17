var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("menudb");

// Add Azure Storage emulator (Azurite)
var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator(c => c.WithBlobPort(10000));

var blobs = storage.AddBlobs("blobs");

// Add Menu API
var menuApi = builder.AddProject("menu-api", "../SmartCafe.Menu.API/SmartCafe.Menu.API.csproj")
    .WithReference(postgres)
    .WithReference(blobs);

builder.Build().Run();
