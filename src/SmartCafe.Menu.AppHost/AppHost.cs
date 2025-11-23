var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("MenuDb");

// Add Azure Storage emulator (Azurite)
var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator(c => c.WithBlobPort(10000));

var blobs = storage.AddBlobs("blobs");

// Add Menu Migrator
var migrator = builder.AddProject<Projects.SmartCafe_Menu_Migrator>("migrator")
    .WithReference(postgres)
    .WaitFor(postgres);

// Add Menu API
var menuApi = builder.AddProject<Projects.SmartCafe_Menu_API>("menu-api")
    .WaitForCompletion(migrator)
    .WithReference(postgres)
    .WithReference(blobs);

builder.Build().Run();
