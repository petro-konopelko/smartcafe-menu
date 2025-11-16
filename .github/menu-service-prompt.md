# Menu Service Backend - Creation Prompt

Use this prompt to create the Menu Service backend project for SmartCafe.

---

## Project Setup Prompt

```
Create a .NET 10 Web API project for the Menu Service using Clean Architecture principles with the following structure:

**Project Name:** SmartCafe.Menu

**Architecture Layers:**
1. Domain - Core business logic and entities
2. Application - Use cases, DTOs, interfaces
3. Infrastructure - Data access, external services, Azure integrations
4. API - Minimal API endpoints, filters, middleware, startup

**Core Features:**

1. **Menu Management:**
   - Each cafe can have **multiple menus** (e.g., Summer Menu, Winter Menu, Holiday Special)
   - Only **one menu is active** at a time
   - Menu states: Draft, Published (Inactive), Active
   - Create new menu (starts as draft)
   - Update menu details (name, sections, items)
   - Delete menu (soft delete - only for unpublished drafts)
   - List all menus (shows draft, published, active status)
   - **Activate menu**: Switch active menu (e.g., activate "Summer Menu" for summer season)
   - Publish menu (makes it available for activation)
   - **Menu grid view**: See all menus at a glance with status badges
   - Clone existing menu to create variations (e.g., copy Summer 2024 to Summer 2025)

2. **Section Management:**
   - Add sections to menus (e.g., breakfast, lunch, dinner)
   - Update section details (name, availability hours)
   - Delete sections
   - Reorder sections within a menu
   - Configure availability hours (from/to time)
   - Maximum 100 items per section

3. **Menu Item Management:**
   - Add items to sections
   - Update item details (name, description, price)
   - Delete items (soft delete)
   - Assign multiple categories to items
   - Upload and manage item images (big and cropped)
   - Configure ingredient options (include/exclude)
   - Set default images when none provided

4. **Category Management:**
   - Create custom categories (beyond defaults: Vegetarian, Spicy)
   - Update category details
   - Delete categories
   - Assign optional icons/images to categories
   - List all categories with filtering

5. **Image Management:**
   - Upload images to Azure Blob Storage
   - Generate cropped versions for fast scrolling
   - Store big image URLs and cropped image URLs
   - Use default images when none provided
   - Delete images from blob storage
   - Support multiple image formats (JPEG, PNG, WebP)

6. **Event Publishing:**
   - Publish events to Azure Service Bus on:
     - Menu created
     - Menu updated
     - Menu deleted
     - Menu published (new version)
     - Item added/updated/deleted
   - Use domain events pattern

**Technology Stack:**
- .NET 10 with C# 14 features
- ASP.NET Core Minimal API for endpoints
- Entity Framework Core 10 for data access (PostgreSQL)
- Npgsql.EntityFrameworkCore.PostgreSQL for PostgreSQL provider
- Azure Blob Storage SDK
- Azure Service Bus SDK
- FluentValidation for input validation
- Mapperly for object mapping (compile-time source generator)
- SixLabors.ImageSharp for image processing
- Serilog for structured logging
- OpenTelemetry for observability (traces, metrics, logs)
- .NET Aspire for local development orchestration

**Database:**
- Use PostgreSQL for relational data storage
- Entity Framework Core for ORM with code-first migrations
- Leverage PostgreSQL JSONB type for flexible nested data (ingredients, metadata)
- Use PostgreSQL indexes and constraints for performance and data integrity
- Configure database transactions for multi-entity operations
- Include seed data for testing (default categories)
- Use foreign key constraints for referential integrity

**Code Quality & Standards:**
- Enable latest .NET analyzers
- Treat warnings as errors: `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`
- Use C# 14 features:
  - Primary constructors
  - Collection expressions
  - Inline arrays where applicable
  - Raw string literals
  - Required members
- Enable nullable reference types
- Use file-scoped namespaces
- Enable implicit usings
- Configure editorconfig for consistent formatting

**API Endpoints (using Minimal API):**
```
# Menu Management
GET    /api/cafes/{cafeId}/menus                    - List all menus (draft, published, active)
POST   /api/cafes/{cafeId}/menus                    - Create new menu (starts as draft)
GET    /api/cafes/{cafeId}/menus/{menuId}           - Get menu with full structure (sections + items)
PUT    /api/cafes/{cafeId}/menus/{menuId}           - Update entire menu structure
DELETE /api/cafes/{cafeId}/menus/{menuId}           - Soft delete (draft menus only)

# Menu Publishing & Activation
POST   /api/cafes/{cafeId}/menus/{menuId}/publish   - Publish menu (makes available for activation)
POST   /api/cafes/{cafeId}/menus/{menuId}/activate  - Activate menu (deactivates previous active menu)

# Public API
GET    /api/cafes/{cafeId}/menu/active              - Get currently active menu for customers

# Menu Operations
POST   /api/cafes/{cafeId}/menus/{menuId}/clone     - Clone menu to create new variation

# Image Management
POST   /api/cafes/{cafeId}/menus/{menuId}/items/{itemId}/image  - Upload item image
DELETE /api/cafes/{cafeId}/menus/{menuId}/items/{itemId}/image  - Delete item image

# Categories (Global)
GET    /api/categories              - List all categories
POST   /api/categories              - Create custom category
PUT    /api/categories/{categoryId} - Update category
DELETE /api/categories/{categoryId} - Delete category
```

**Configuration Requirements:**
- appsettings.json with non-sensitive settings:
  - PostgreSQL connection string template (without password)
  - Azure Blob Storage account name
  - Azure Service Bus namespace
  - Database name, host, port, username
- **Secrets Management**:
  - **Development**: User Secrets or environment variables
  - **Production**: Azure Key Vault or environment variables
  - Never store passwords/keys in appsettings.json
  - PostgreSQL password from environment variable: `ConnectionStrings__PostgreSQL__Password`
  - Azure Blob Storage key from Key Vault or env var
  - Azure Service Bus connection string from Key Vault or env var
- Blob storage container configuration
- Image processing settings (max size, allowed formats, thumbnail dimensions)
- Environment-specific configs (Development, Staging, Production)
- Health checks for PostgreSQL, Blob Storage, Service Bus
- OpenTelemetry configuration:
  - OTLP endpoint
  - Service name and version
  - Trace, metric, and log exporters
- .NET Aspire AppHost for local orchestration

**Additional Requirements:**
- Use Minimal API with endpoint filters for cross-cutting concerns
- Use repository pattern with EF Core for data access
- Implement endpoint filters for validation, logging, and exception handling
- Add comprehensive structured logging with Serilog
- Include Swagger/OpenAPI documentation with .WithOpenApi()
- Add correlation IDs for distributed request tracking
- Implement global exception handling
- Use API versioning (v1)
- Add CORS configuration
- Include Docker support (Dockerfile with multi-stage build)
- Implement caching strategy for published menus (distributed cache)
- Configure OpenTelemetry with:
  - Activity-based tracing
  - Custom metrics (menu operations, image uploads)
  - Structured log correlation
  - EF Core instrumentation
- Create .NET Aspire AppHost project for local development
- Use database transactions for consistency

**Validation Rules:**
- Cafe can only have **one active menu** at a time (enforced by unique index)
- Menu must have at least one section
- Section names must be unique within a menu
- Maximum 100 items per section
- Item prices must be positive
- Availability hours must be valid (from < to)
- Image file size limits (max 5MB)
- Supported image formats: JPEG, PNG, WebP
- At least one category must be assigned to each item
- Ingredient options must be valid
- When publishing a new menu version:
  - Previous active menu must be deactivated
  - Version number increments automatically
  - Published menus become immutable

**Domain Entities (PostgreSQL Tables):**
```csharp
// Use Guid with CreateVersion7() for time-ordered globally unique identifiers
// All DateTime properties should be in UTC and set via IDateTimeProvider
public class Cafe
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required string Name { get; init; }
    public string? ContactInfo { get; init; }
    public DateTime CreatedAt { get; init; } // Set via IDateTimeProvider.UtcNow
}

public class Menu
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid CafeId { get; init; }
    public Cafe Cafe { get; init; } = null!; // Navigation property
    public required string Name { get; init; } // e.g., "Summer Menu 2025", "Winter Menu"
    public bool IsActive { get; set; } // Only one active menu per cafe at a time
    public bool IsPublished { get; set; } // Must be published before it can be activated
    public DateTime? PublishedAt { get; set; }
    public DateTime? ActivatedAt { get; set; } // When this menu became active
    public List<Section> Sections { get; init; } = [];
    public DateTime CreatedAt { get; init; } // Set via IDateTimeProvider.UtcNow
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } // Soft delete for drafts only
}

public class Section
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid MenuId { get; init; }
    public Menu Menu { get; init; } = null!; // Navigation property
    public required string Name { get; init; }
    public int DisplayOrder { get; set; }
    public TimeSpan? AvailableFrom { get; init; }
    public TimeSpan? AvailableTo { get; init; }
    public ICollection<MenuItem> Items { get; init; } = [];
    public DateTime CreatedAt { get; init; } // Set via IDateTimeProvider.UtcNow
}

public class MenuItem
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid SectionId { get; init; }
    public Section Section { get; init; } = null!; // Navigation property
    public required string Name { get; init; }
    public required string Description { get; init; }
    public decimal Price { get; init; }
    public string? ImageBigUrl { get; set; }
    public string? ImageCroppedUrl { get; set; }
    public ICollection<MenuItemCategory> MenuItemCategories { get; init; } = []; // Many-to-many
    public List<Ingredient> IngredientOptions { get; init; } = []; // Stored as JSONB
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; init; } // Set via IDateTimeProvider.UtcNow
    public DateTime UpdatedAt { get; set; } // Set via IDateTimeProvider.UtcNow
}

public class Category
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public required string Name { get; init; }
    public string? IconUrl { get; set; }
    public bool IsDefault { get; init; }
    public ICollection<MenuItemCategory> MenuItemCategories { get; init; } = [];
    public DateTime CreatedAt { get; init; } // Set via IDateTimeProvider.UtcNow
}

// Join table for many-to-many relationship
public class MenuItemCategory
{
    public Guid MenuItemId { get; init; }
    public MenuItem MenuItem { get; init; } = null!;
    public Guid CategoryId { get; init; }
    public Category Category { get; init; } = null!;
}

public class Ingredient
{
    public required string Name { get; init; }
    public bool IsExcludable { get; init; }
    public bool IsIncludable { get; init; }
}
```

**IDateTimeProvider Interface:**
```csharp
// Domain or Application layer interface for testable time operations
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
    DateTime Today { get; }
    DateTimeOffset UtcNowOffset { get; }
    DateTimeOffset NowOffset { get; }
}

// Infrastructure layer implementation
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Now => DateTime.Now;
    public DateTime Today => DateTime.Today;
    public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;
    public DateTimeOffset NowOffset => DateTimeOffset.Now;
}

// Usage in services and repositories
public class MenuRepository(MenuDbContext context, IDateTimeProvider dateTimeProvider) : IMenuRepository
{
    public async Task<Menu> CreateAsync(Menu menu, CancellationToken ct)
    {
        // All timestamps use UTC via provider
        var now = dateTimeProvider.UtcNow;
        
        var entity = menu with 
        { 
            CreatedAt = now,
            UpdatedAt = now
        };
        
        context.Menus.Add(entity);
        await context.SaveChangesAsync(ct);
        return entity;
    }
}

// Mock for testing
public class FakeDateTimeProvider : IDateTimeProvider
{
    private DateTime _utcNow = DateTime.UtcNow;
    
    public DateTime UtcNow => _utcNow;
    public DateTime Now => _utcNow.ToLocalTime();
    public DateTime Today => _utcNow.Date;
    public DateTimeOffset UtcNowOffset => new DateTimeOffset(_utcNow);
    public DateTimeOffset NowOffset => new DateTimeOffset(_utcNow.ToLocalTime());
    
    public void SetUtcNow(DateTime utcNow) => _utcNow = utcNow;
}
```

**Important DateTime Guidelines:**
- **Always use UTC**: All DateTime values stored in database must be UTC
- **Use IDateTimeProvider**: Never use `DateTime.UtcNow` directly in business logic
- **Testability**: IDateTimeProvider allows freezing time in tests
- **PostgreSQL configuration**: Configure EF Core to store DateTime as `timestamp with time zone`
- **API responses**: Return UTC timestamps; let clients convert to local time
- **Validation**: Ensure incoming DateTime values are converted to UTC

**Domain Events to Publish:**
- MenuCreatedEvent
- MenuUpdatedEvent
- MenuDeletedEvent
- MenuPublishedEvent (with version number)
- ItemCreatedEvent
- ItemUpdatedEvent
- ItemDeletedEvent

Each event should include: eventId, menuId, cafeId, timestamp, eventType, version (if applicable), and relevant payload.

**Image Processing Requirements:**
- Generate cropped/thumbnail version on upload (e.g., 400x300px)
- Store original as "big" image
- Use consistent naming: {itemId}_big.{ext} and {itemId}_cropped.{ext}
- Clean up blob storage when images are deleted
- Support progressive loading in frontend
- Use SixLabors.ImageSharp for efficient processing

**Mapperly Configuration:**
- Use Mapperly source generator for compile-time mapping
- Create partial mapper classes with [Mapper] attribute
- Configure custom mappings where needed
- No runtime reflection overhead
- Full type safety at compile time

**OpenTelemetry Configuration:**
- Configure ActivitySource for distributed tracing
- Add custom metrics:
  - Menu creation/update counters
  - Image upload size and duration
  - EF Core query duration and count
  - Cache hit/miss ratios
- Structured logging with Serilog and OTLP exporter
- Correlation of traces, metrics, and logs
- Export to OTLP endpoint (configurable)

**.NET Aspire Setup:**
- Create SmartCafe.Menu.AppHost project
- Configure PostgreSQL resource (using Aspire.Hosting.PostgreSQL)
- Configure Azure Blob Storage emulator (Azurite)
- Configure Azure Service Bus emulator or test instance
- Setup service discovery
- Configure health checks dashboard
- Environment variable management
- Configure EF Core migrations on startup

**Testing:**
- Include xUnit test project
- Unit tests for domain logic
- Integration tests for Minimal API endpoints using WebApplicationFactory
- Use Testcontainers for PostgreSQL in integration tests
- Use Testcontainers for Azurite (Blob Storage emulator)
- Mock Azure Service Bus for testing
- **Mock IDateTimeProvider** for time-dependent tests (freeze time, test time boundaries)
- Test endpoint filters (validation, exception handling)
- Test OpenTelemetry instrumentation
- Test EF Core migrations
- Performance tests for API endpoints

**Example Test with IDateTimeProvider:**
```csharp
public class MenuServiceTests
{
    [Fact]
    public async Task CreateMenu_SetsCorrectTimestamp()
    {
        // Arrange
        var fixedTime = new DateTime(2025, 11, 16, 10, 30, 0, DateTimeKind.Utc);
        var dateTimeProvider = new FakeDateTimeProvider();
        dateTimeProvider.SetUtcNow(fixedTime);
        
        var repository = new MenuRepository(dbContext, dateTimeProvider);
        
        // Act
        var menu = await repository.CreateAsync(new Menu { ... });
        
        // Assert
        Assert.Equal(fixedTime, menu.CreatedAt);
        Assert.Equal(DateTimeKind.Utc, menu.CreatedAt.Kind);
    }
}
```

**Project Structure:**
```
src/
├── SmartCafe.Menu.AppHost/              # .NET Aspire orchestration
│   ├── Program.cs
│   └── appsettings.json
├── SmartCafe.Menu.ServiceDefaults/      # Shared Aspire configuration
│   ├── Extensions.cs
│   └── HostingExtensions.cs
├── SmartCafe.Menu.Domain/
│   ├── Entities/
│   ├── Enums/
│   ├── Events/
│   ├── Exceptions/
│   └── ValueObjects/
├── SmartCafe.Menu.Application/
│   ├── Features/                        # Vertical slice architecture
│   │   ├── Menus/
│   │   │   ├── CreateMenu/
│   │   │   ├── UpdateMenu/
│   │   │   ├── PublishMenu/
│   │   │   └── GetMenu/
│   │   ├── Sections/
│   │   ├── Items/
│   │   └── Categories/
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Mappings/                        # Mapperly mapper classes
│   └── Validators/                      # FluentValidation
├── SmartCafe.Menu.Infrastructure/
│   ├── Data/
│   │   └── PostgreSQL/
│   │       ├── MenuDbContext.cs
│   │       ├── Configurations/           # EF Core entity configurations
│   │       ├── Migrations/               # EF Core migrations
│   │       └── UnitOfWork.cs
│   ├── Repositories/
│   ├── EventBus/
│   │   └── ServiceBusPublisher.cs
│   ├── BlobStorage/
│   │   └── AzureBlobService.cs
│   └── Services/
│       └── ImageProcessingService.cs
└── SmartCafe.Menu.API/
    ├── Endpoints/                       # Minimal API endpoint extensions
    │   ├── Menus/
    │   │   ├── CreateMenuEndpoint.cs
    │   │   ├── GetMenuEndpoint.cs
    │   │   ├── UpdateMenuEndpoint.cs
    │   │   ├── DeleteMenuEndpoint.cs
    │   │   ├── PublishMenuEndpoint.cs
    │   │   └── ListMenusEndpoint.cs
    │   ├── Sections/
    │   │   ├── AddSectionEndpoint.cs
    │   │   ├── UpdateSectionEndpoint.cs
    │   │   ├── DeleteSectionEndpoint.cs
    │   │   └── ReorderSectionsEndpoint.cs
    │   ├── Items/
    │   │   ├── AddItemEndpoint.cs
    │   │   ├── UpdateItemEndpoint.cs
    │   │   ├── DeleteItemEndpoint.cs
    │   │   └── UploadImageEndpoint.cs
    │   ├── Categories/
    │   │   ├── CreateCategoryEndpoint.cs
    │   │   ├── UpdateCategoryEndpoint.cs
    │   │   └── DeleteCategoryEndpoint.cs
    │   └── EndpointExtensions.cs        # Central endpoint registration
    ├── Filters/
    │   ├── ValidationFilter.cs
    │   ├── ExceptionFilter.cs
    │   └── LoggingFilter.cs
    ├── Middleware/
    ├── Program.cs
    └── appsettings.json

tests/
├── SmartCafe.Menu.UnitTests/
│   ├── Domain/
│   ├── Application/
│   └── Infrastructure/
└── SmartCafe.Menu.IntegrationTests/
    ├── Endpoints/
    └── Fixtures/
        └── PostgreSqlFixture.cs
```

**PostgreSQL Indexes and Constraints:**
Create indexes and constraints for performance and data integrity:
```sql
-- Note: All Id columns are uuid type
-- Foreign key constraints (auto-created by EF Core)
ALTER TABLE Menus ADD CONSTRAINT FK_Menus_Cafes FOREIGN KEY (CafeId) REFERENCES Cafes(Id);
ALTER TABLE Sections ADD CONSTRAINT FK_Sections_Menus FOREIGN KEY (MenuId) REFERENCES Menus(Id) ON DELETE CASCADE;
ALTER TABLE MenuItems ADD CONSTRAINT FK_MenuItems_Sections FOREIGN KEY (SectionId) REFERENCES Sections(Id) ON DELETE CASCADE;

-- Performance indexes (GUIDs work efficiently with B-tree indexes)
CREATE INDEX IX_Menus_CafeId ON Menus(CafeId);
CREATE INDEX IX_Menus_CafeId_IsDeleted ON Menus(CafeId, IsDeleted); -- For listing active menus
CREATE INDEX IX_Menus_CafeId_IsActive ON Menus(CafeId, IsActive);
CREATE INDEX IX_Menus_CafeId_IsPublished ON Menus(CafeId, IsPublished);

-- Unique partial index: Only one active menu per cafe
CREATE UNIQUE INDEX UX_Menus_CafeId_Active ON Menus(CafeId) WHERE IsActive = true AND IsDeleted = false;

CREATE INDEX IX_Sections_MenuId ON Sections(MenuId);
CREATE INDEX IX_MenuItems_SectionId ON MenuItems(SectionId);
CREATE INDEX IX_Categories_IsDefault ON Categories(IsDefault);

-- Clustered index pattern for time-ordered queries (leverages CreateVersion7's time ordering)
CREATE INDEX IX_Menus_CafeId_CreatedAt ON Menus(CafeId, CreatedAt DESC);
CREATE INDEX IX_MenuItems_CreatedAt ON MenuItems(CreatedAt DESC);

-- Full-text search index
CREATE INDEX IX_MenuItems_FullText ON MenuItems USING gin(to_tsvector('english', Name || ' ' || Description));

-- JSONB index for ingredient queries
CREATE INDEX IX_MenuItems_IngredientOptions ON MenuItems USING gin(IngredientOptions);

-- Check constraints
ALTER TABLE MenuItems ADD CONSTRAINT CK_MenuItems_Price_Positive CHECK (Price > 0);
ALTER TABLE Sections ADD CONSTRAINT CK_Sections_AvailableHours CHECK (AvailableFrom IS NULL OR AvailableTo IS NULL OR AvailableFrom < AvailableTo);
```

**Why Guid.CreateVersion7()?**
- Available in .NET 9+ (use in .NET 10 project)
- Generates time-ordered UUIDs (UUIDv7 spec)
- Better database index performance than random GUIDs
- Maintains chronological ordering for range queries
- Prevents enumeration attacks (unlike sequential integers)
- Globally unique across distributed systems

**Seed Data:**
Create default categories using EF Core migrations or data seeding:
```csharp
// In DbContext OnModelCreating or separate seeding logic
// Note: For seed data, using a fixed UTC timestamp is acceptable
var seedTime = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
var vegetarianId = Guid.Parse("550e8400-e29b-41d4-a716-446655440001");
var spicyId = Guid.Parse("550e8400-e29b-41d4-a716-446655440002");

modelBuilder.Entity<Category>().HasData(
    new Category { Id = vegetarianId, Name = "Vegetarian", IsDefault = true, CreatedAt = seedTime },
    new Category { Id = spicyId, Name = "Spicy", IsDefault = true, CreatedAt = seedTime }
);
```

**Minimal API Example:**
```csharp
// Endpoint definition in Endpoints/Menus/CreateMenuEndpoint.cs
public static class CreateMenuEndpoint
{
    public static RouteGroupBuilder MapCreateMenu(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            Guid cafeId,
            CreateMenuRequest request,
            IMenuRepository repository,
            IUnitOfWork unitOfWork,
            IValidator<CreateMenuRequest> validator,
            IDateTimeProvider dateTimeProvider,
            CancellationToken ct) =>
        {
            // Validation
            var validationResult = await validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            // Business logic and mapping
            var menu = request.ToMenu(cafeId); // Mapperly generated
            await repository.CreateAsync(menu, ct);
            await unitOfWork.SaveChangesAsync(ct);
            
            var response = menu.ToResponse();
            return Results.Created($"/api/cafes/{cafeId}/menus/{menu.Id}", response);
        })
        .WithName("CreateMenu")
        .WithSummary("Create a new menu for a cafe")
        .WithOpenApi()
        .Produces<MenuResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .AddEndpointFilter<LoggingFilter>();
        
        return group;
    }
}

// Registration in Program.cs
var cafes = app.MapGroup("/api/cafes/{cafeId:int}").WithOpenApi();
var menus = cafes.MapGroup("/menus");
menus.MapCreateMenu();
menus.MapGetMenu();
menus.MapUpdateMenu();
// ...
```

**Minimal API Configuration:**
- Use route groups to organize related endpoints
- Apply common metadata with .WithOpenApi() and .WithTags()
- Use endpoint filters for cross-cutting concerns (validation, logging)
- Leverage parameter binding (route, query, body, services)
- Use Results.* methods for consistent responses
- Configure Swagger/OpenAPI with endpoint descriptions
- Use typed results for better documentation
- Enable ProblemDetails for standardized error responses

**Program.cs Structure:**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Configure secrets (Key Vault in production, User Secrets in dev)
if (builder.Environment.IsProduction())
{
    var keyVaultUri = builder.Configuration["KeyVault:Uri"];
    if (!string.IsNullOrEmpty(keyVaultUri))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential());
    }
}

// Build PostgreSQL connection string dynamically from configuration + secrets
var connectionString = BuildPostgreSqlConnectionString(builder.Configuration);

// Add services
builder.Services.AddDbContext<MenuDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateMenuValidator>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();

// Map endpoints
var api = app.MapGroup("/api").WithOpenApi();
var cafes = api.MapGroup("/cafes/{cafeId:guid}");
var menus = cafes.MapGroup("/menus");

menus.MapCreateMenu();
menus.MapGetMenu();
menus.MapUpdateMenu();
menus.MapDeleteMenu();
menus.MapPublishMenu();
// ... other endpoints

app.Run();

// Helper method to build connection string dynamically
static string BuildPostgreSqlConnectionString(IConfiguration configuration)
{
    var host = configuration["Database:Host"] ?? "localhost";
    var port = configuration["Database:Port"] ?? "5432";
    var database = configuration["Database:Name"] ?? "smartcafe_menu";
    var username = configuration["Database:Username"] ?? "postgres";
    
    // Password from environment variable or Key Vault (never from appsettings.json)
    var password = configuration["Database:Password"] 
        ?? configuration["ConnectionStrings:PostgreSQL:Password"]
        ?? Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")
        ?? throw new InvalidOperationException("Database password not configured");
    
    return $"Host={host};Port={port};Database={database};Username={username};Password={password};Include Error Detail=true";
}
```

**appsettings.json Example:**
```json
{
  "Database": {
    "Host": "localhost",
    "Port": "5432",
    "Name": "smartcafe_menu",
    "Username": "postgres"
    // Password is NOT stored here - comes from environment or Key Vault
  },
  "AzureStorage": {
    "AccountName": "smartcafestorage",
    "ContainerName": "menu-images"
    // Key comes from environment or Key Vault
  },
  "AzureServiceBus": {
    "Namespace": "smartcafe.servicebus.windows.net"
    // Connection string comes from environment or Key Vault
  },
  "KeyVault": {
    "Uri": "https://smartcafe-keyvault.vault.azure.net/"
  },
  "ImageProcessing": {
    "MaxFileSizeBytes": 5242880,
    "AllowedFormats": ["jpg", "jpeg", "png", "webp"],
    "ThumbnailWidth": 400,
    "ThumbnailHeight": 300
  },
  "OpenTelemetry": {
    "ServiceName": "SmartCafe.Menu",
    "ServiceVersion": "1.0.0",
    "OtlpEndpoint": "http://localhost:4317"
  }
}
```

**appsettings.Production.json Example:**
```json
{
  "Database": {
    "Host": "smartcafe-postgres.postgres.database.azure.com",
    "Port": "5432",
    "Name": "smartcafe_menu_prod",
    "Username": "smartcafe_admin"
  },
  "KeyVault": {
    "Uri": "https://smartcafe-prod-kv.vault.azure.net/"
  }
}
```

**Environment Variables (Development):**
```bash
# .env or User Secrets
DATABASE__PASSWORD=your_dev_password
AZURESTO RAGE__KEY=your_storage_key
AZURESERVICEBUS__CONNECTIONSTRING=your_servicebus_connection
```

**Azure Key Vault Secrets (Production):**
```
Database--Password: <secure_postgres_password>
AzureStorage--Key: <storage_account_key>
AzureServiceBus--ConnectionString: <servicebus_connection_string>
```

**Docker/Kubernetes Environment Variables:**
```yaml
apiVersion: v1
kind: Secret
metadata:
  name: menu-service-secrets
type: Opaque
stringData:
  POSTGRES_PASSWORD: <base64-encoded>
  AZURE_STORAGE_KEY: <base64-encoded>
  AZURE_SERVICEBUS_CONNECTIONSTRING: <base64-encoded>
```

**EF Core Configuration:**
- Use fluent API in `MenuDbContext.OnModelCreating` for entity configuration
- Configure relationships (one-to-many, many-to-many)
- Configure JSONB columns for `IngredientOptions` using `.HasColumnType("jsonb")`
- Configure Guid properties to use PostgreSQL uuid type (default behavior)
- Use `Guid.CreateVersion7()` for ID generation (C# side, not database)
- **Configure DateTime properties as UTC**: Use `timestamp with time zone` PostgreSQL type
- Enable query splitting for collections to avoid cartesian explosion
- Configure value converters for `TimeSpan` to `time` type
- Enable lazy loading proxies or use explicit/eager loading
- Configure cascade delete behaviors
- Use global query filters for soft deletes

**Example EF Core Configuration:**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Guid configuration (uses PostgreSQL uuid type by default with Npgsql)
    modelBuilder.Entity<Menu>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id).ValueGeneratedNever(); // C# generates via CreateVersion7()
        
        // DateTime configuration - ensure UTC storage
        entity.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone");
        entity.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp with time zone");
        entity.Property(e => e.PublishedAt)
            .HasColumnType("timestamp with time zone");
        entity.Property(e => e.ActivatedAt)
            .HasColumnType("timestamp with time zone");
        
        entity.HasOne(e => e.Cafe)
            .WithMany()
            .HasForeignKey(e => e.CafeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Index for common query patterns
        entity.HasIndex(e => new { e.CafeId, e.CreatedAt });
        entity.HasIndex(e => new { e.CafeId, e.IsDeleted });
        
        // Unique partial index: Only one active menu per cafe
        entity.HasIndex(e => e.CafeId)
            .IsUnique()
            .HasFilter("\"IsActive\" = true AND \"IsDeleted\" = false")
            .HasDatabaseName("UX_Menus_CafeId_Active");
            
        // Global query filter for soft delete
        entity.HasQueryFilter(e => !e.IsDeleted);
    });
    
    // JSONB configuration
    modelBuilder.Entity<MenuItem>(entity =>
    {
        entity.Property(e => e.IngredientOptions)
            .HasColumnType("jsonb");
            
        // DateTime UTC configuration
        entity.Property(e => e.CreatedAt)
            .HasColumnType("timestamp with time zone");
        entity.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp with time zone");
    });
    
    // Many-to-many configuration
    modelBuilder.Entity<MenuItemCategory>()
        .HasKey(mc => new { mc.MenuItemId, mc.CategoryId });
        
    // Global configuration for all DateTime properties to use UTC
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        foreach (var property in entityType.GetProperties())
        {
            if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
            {
                property.SetColumnType("timestamp with time zone");
            }
        }
    }
}

// Configure Npgsql to handle DateTime as UTC globally
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.UseNodaTime(); // Optional: Use NodaTime for better timezone handling
    });
}
```

Please generate the complete project structure with all necessary files, configurations, and initial implementations following:
- Clean Architecture principles
- SOLID principles
- Vertical slice architecture for features
- ASP.NET Core Minimal API for endpoints
- Endpoint filters for cross-cutting concerns
- PostgreSQL with Entity Framework Core for data storage
- **IDateTimeProvider for testable time operations** (all DateTime values in UTC)
- Mapperly for compile-time mapping
- FluentValidation for input validation
- OpenTelemetry for observability
- .NET Aspire for local development
- C# 14 features throughout (primary constructors, collection expressions, etc.)
- Warnings as errors
- Code-first EF Core migrations
- WebApplicationFactory for integration testing
- PostgreSQL `timestamp with time zone` for all DateTime columns

**Security & Secrets Management Best Practices:**
- **Never commit secrets** to source control (use .gitignore for appsettings.Development.json with secrets)
- **Use User Secrets** for local development: `dotnet user-secrets set "Database:Password" "dev_password"`
- **Use Azure Key Vault** for production secrets with Managed Identity authentication
- **Use environment variables** as fallback (Docker, Kubernetes)
- **Validate configuration** on startup to fail fast if secrets are missing
- **Rotate secrets** regularly and update Key Vault
- **Use connection pooling** with secure connection strings
- **Enable SSL/TLS** for PostgreSQL connections in production
- **Principle of least privilege** for database user permissions
