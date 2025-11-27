# SmartCafe Menu Service

[![CI](https://github.com/petro-konopelko/smartcafe-menu/actions/workflows/ci.yaml/badge.svg)](https://github.com/petro-konopelko/smartcafe-menu/actions/workflows/ci.yaml)
[![PR](https://github.com/petro-konopelko/smartcafe-menu/actions/workflows/pr.yaml/badge.svg)](https://github.com/petro-konopelko/smartcafe-menu/actions/workflows/pr.yaml)

A .NET 10 microservice for managing digital menus in the SmartCafe smart ordering system. Built with Clean Architecture, PostgreSQL, and Azure services.

## 🏗️ Architecture

- **Clean Architecture** with Vertical Slice Architecture
- **Result Pattern**: Zero exception-based error handling in application layer
- **Domain Layer**: Entities, value objects, domain events
- **Application Layer**: Handlers (return `Result<T>`), DTOs, validators, Mapperly mappers
- **Infrastructure Layer**: EF Core, PostgreSQL, Azure Blob Storage, Azure Service Bus
- **API Layer**: ASP.NET Core Minimal API with Result extensions

## 🚀 Features

- **Multi-Menu Management**: Cafes can have multiple menus (e.g., Summer, Winter, Holiday)
- **Menu States**: Draft → Published → Active (only one active menu per cafe)
- **Menu Activation**: Switch active menus seamlessly
- **Menu Cloning**: Create variations from existing menus
- **Section Management**: Organize items by meal type with availability hours
- **Item Management**: Full CRUD with categories, images, and ingredients
- **Image Processing**: Auto-generate cropped thumbnails
- **Event Publishing**: Publish domain events to Azure Service Bus
- **Time-Ordered GUIDs**: Use `Guid.CreateVersion7()` for better database performance

## 🛠️ Tech Stack

- **.NET 10** with C# 14 (latest language features)
- **ASP.NET Core Minimal API**
- **Entity Framework Core 10** with PostgreSQL
- **FluentValidation** for input validation
- **Mapperly** for compile-time mapping
- **Azure Blob Storage** for images
- **Azure Service Bus** for events
- **Serilog** for structured logging
- **OpenTelemetry** for observability
- **.NET Aspire** for local development

## 📋 Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL 16+](https://www.postgresql.org/download/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for local development)
- [Azure Storage Emulator (Azurite)](https://learn.microsoft.com/azure/storage/common/storage-use-azurite)
- Visual Studio 2025 or VS Code with C# Dev Kit

## 🏃‍♂️ Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/petro-konopelko/smartcafe-menu.git
cd smartcafe-menu
```

### 2. Set Up User Secrets

```bash
cd src/SmartCafe.Menu.API
dotnet user-secrets init
dotnet user-secrets set "Database:Password" "your_postgres_password"
dotnet user-secrets set "AzureStorage:Key" "your_storage_key"
dotnet user-secrets set "AzureServiceBus:ConnectionString" "your_servicebus_connection"
```

### 3. Run with .NET Aspire

```bash
cd src/SmartCafe.Menu.AppHost
dotnet run
```

This will start:
- Menu API service
- PostgreSQL database
- Azurite (Azure Storage Emulator)
- Aspire Dashboard (http://localhost:15888)

### 4. Apply Database Migrations

```bash
cd src/SmartCafe.Menu.Infrastructure
dotnet ef database update --startup-project ../SmartCafe.Menu.API
```

### 5. Access the API

- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **Aspire Dashboard**: http://localhost:15888

## 📁 Project Structure

```
smartcafe-menu/
├── src/
│   ├── SmartCafe.Menu.Domain/           # Core business logic
│   │   ├── Entities/                    # Domain entities
│   │   ├── ValueObjects/                # Value objects (Ingredient)
│   │   ├── Events/                      # Domain events
│   │   ├── Exceptions/                  # Custom exceptions
│   │   └── Interfaces/                  # IDateTimeProvider
│   ├── SmartCafe.Menu.Application/      # Use cases & DTOs
│   │   ├── Common/Results/              # Result pattern (Result<T>, Error, ErrorType)
│   │   ├── Features/                    # Vertical slices (handlers return Result<T>)
│   │   │   ├── Menus/
│   │   │   └── Categories/
│   │   ├── Interfaces/                  # Repository interfaces
│   │   ├── Mappings/                    # Mapperly mappers
│   │   └── Mediation/                   # Mediator, ValidationBehavior
│   ├── SmartCafe.Menu.Infrastructure/   # Data access & external services
│   │   ├── Data/PostgreSQL/             # EF Core DbContext
│   │   ├── Repositories/                # Repository implementations
│   │   ├── EventBus/                    # Azure Service Bus
│   │   ├── BlobStorage/                 # Azure Blob Storage
│   │   └── Services/                    # DateTimeProvider, ImageProcessing
│   ├── SmartCafe.Menu.API/              # Minimal API endpoints
│   │   ├── Endpoints/                   # Endpoint definitions (use Result extensions)
│   │   ├── Extensions/                  # Result → HTTP mapping (ToApiResult, ToCreatedResult)
│   │   ├── Filters/                     # Validation, logging filters
│   │   ├── Middleware/                  # Exception handling for unexpected errors
│   │   └── Program.cs                   # Application startup
│   ├── SmartCafe.Menu.AppHost/          # .NET Aspire orchestration
│   └── SmartCafe.Menu.ServiceDefaults/  # Shared Aspire config
├── tests/
│   ├── SmartCafe.Menu.UnitTests/
│   └── SmartCafe.Menu.IntegrationTests/
├── .editorconfig
├── .gitignore
└── SmartCafe.Menu.sln
```

## 🔑 Key Design Patterns

### Result Pattern (No Exceptions)

All handlers return `Result<T>` instead of throwing exceptions:

```csharp
public class CreateMenuHandler : ICommandHandler<CreateMenuRequest, Result<CreateMenuResponse>>
{
    public async Task<Result<CreateMenuResponse>> HandleAsync(...)
    {
        // Return errors instead of throwing
        if (menu == null)
            return Result<CreateMenuResponse>.Failure(Error.NotFound(
                "Menu not found", "MENU_NOT_FOUND"));
        
        // Return success with value
        return Result<CreateMenuResponse>.Success(new CreateMenuResponse(...));
    }
}
```

**Endpoints use Result extensions:**
```csharp
group.MapPost("/", async (CreateMenuRequest request, IMediator mediator, CancellationToken ct) => 
{
    var result = await mediator.Send<CreateMenuRequest, Result<CreateMenuResponse>>(request, ct);
    return result.ToCreatedResult($"/api/menus/{result.Value!.Id}"); // Auto-maps errors to HTTP status
})
.WithName("CreateMenu")
.WithOpenApi();
```

**Error Types:**
- `Error.NotFound()` → 404 Not Found
- `Error.Validation()` → 400 Bad Request
- `Error.Conflict()` → 409 Conflict

### IDateTimeProvider

All DateTime operations use `IDateTimeProvider` instead of `DateTime.UtcNow` for testability:

```csharp
public class CreateMenuHandler(IMenuRepository repository, IDateTimeProvider dateTimeProvider)
{
    public async Task<Result<CreateMenuResponse>> HandleAsync(CreateMenuRequest request)
    {
        var menu = new Menu
        {
            Name = request.Name,
            CreatedAt = dateTimeProvider.UtcNow, // Testable!
            UpdatedAt = dateTimeProvider.UtcNow
        };
        // ...
    }
}
```

### Guid.CreateVersion7()

Time-ordered UUIDs for better database performance:

```csharp
public class Menu
{
    public Guid Id { get; init; } = Guid.CreateVersion7(); // UUIDv7
}
```

## 🔐 Security & Secrets

- **Development**: Use User Secrets or environment variables
- **Production**: Use Azure Key Vault with Managed Identity
- **Never** store passwords in `appsettings.json`

## 📊 Database Schema

### Key Tables
- `Cafes` - Cafe information
- `Menus` - Menu definitions with state (Draft/Published/Active)
- `Sections` - Menu sections (Breakfast, Lunch, etc.)
- `MenuItems` - Individual menu items
- `Categories` - Item categories (Vegetarian, Spicy, custom)
- `MenuItemCategories` - Many-to-many join table

### Important Constraints
- **Unique partial index**: Only one active menu per cafe
- **Foreign keys**: Cascade delete for menu hierarchies
- **Check constraints**: Price > 0, AvailableFrom < AvailableTo
- **JSONB**: Ingredient options stored as JSONB for flexibility

## 🧪 Testing

```bash
# Run unit tests
dotnet test tests/SmartCafe.Menu.UnitTests

# Run integration tests
dotnet test tests/SmartCafe.Menu.IntegrationTests

# Run all tests
dotnet test
```

## 🐳 Docker

```bash
# Build image
docker build -t smartcafe-menu:latest .

# Run container
docker run -p 5000:8080 smartcafe-menu:latest
```

## 📖 API Documentation

### Menu Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/cafes/{cafeId}/menus` | List all menus |
| POST | `/api/cafes/{cafeId}/menus` | Create new menu (draft) |
| GET | `/api/cafes/{cafeId}/menus/{menuId}` | Get menu details |
| PUT | `/api/cafes/{cafeId}/menus/{menuId}` | Update menu |
| DELETE | `/api/cafes/{cafeId}/menus/{menuId}` | Delete menu (draft only) |
| POST | `/api/cafes/{cafeId}/menus/{menuId}/publish` | Publish menu |
| POST | `/api/cafes/{cafeId}/menus/{menuId}/activate` | Activate menu |
| POST | `/api/cafes/{cafeId}/menus/{menuId}/clone` | Clone menu |
| GET | `/api/cafes/{cafeId}/menu/active` | Get active menu (public) |

See [Swagger UI](http://localhost:5000/swagger) for complete API documentation.

## 🏛️ Clean Architecture Implementation

### Handler Pattern
All endpoints follow the **Handler Pattern** for separation of concerns:

**Endpoint Responsibilities** (thin, 20-50 lines):
- Validate input using FluentValidation
- Call handler with parsed parameters
- Map handler results to HTTP responses
- Catch exceptions and return appropriate status codes

**Handler Responsibilities** (business logic):
- Execute business logic
- Coordinate repositories and services
- Validate business rules
- Publish domain events
- Return DTOs

**Example**:
```csharp
// Endpoint (thin layer)
public static RouteGroupBuilder MapPublishMenu(this RouteGroupBuilder group)
{
    group.MapPost("/{menuId:guid}/publish", async (
        Guid cafeId,
        Guid menuId,
        PublishMenuHandler handler,
        CancellationToken ct) =>
    {
        try
        {
            var result = await handler.HandleAsync(cafeId, menuId, ct);
            return Results.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    })
    .WithName("PublishMenu")
    .Produces<PublishMenuResponse>(StatusCodes.Status200OK);
    
    return group;
}

// Handler (business logic)
public class PublishMenuHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    IDateTimeProvider dateTimeProvider)
{
    public async Task<PublishMenuResponse> HandleAsync(
        Guid cafeId, Guid menuId, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.GetByIdAsync(menuId, cancellationToken);
        
        if (menu == null || menu.CafeId != cafeId)
            throw new InvalidOperationException("Menu not found");
            
        if (menu.IsPublished)
            throw new InvalidOperationException("Menu is already published");
            
        if (menu.Sections.Count == 0)
            throw new InvalidOperationException("Menu must have at least one section");
        
        menu.IsPublished = true;
        menu.PublishedAt = dateTimeProvider.UtcNow;
        
        await menuRepository.UpdateAsync(menu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        await eventPublisher.PublishAsync(
            new MenuPublishedEvent(Guid.CreateVersion7(), menuId, cafeId, menu.Name, dateTimeProvider.UtcNow),
            cancellationToken);
        
        return new PublishMenuResponse(menu.Id, menu.Name, menu.IsPublished, menu.PublishedAt);
    }
}
```

### Implemented Handlers

**Menu Handlers**:
- `UpsertMenuHandler` - Create or update complete menu structure
- `ActivateMenuHandler` - Activate a published menu
- `PublishMenuHandler` - Publish a draft menu
- `DeleteMenuHandler` - Delete draft menus with images
- `GetMenuHandler` - Retrieve menu details
- `ListMenusHandler` - List all menus for a cafe
- `GetActiveMenuHandler` - Get currently active menu

**Category Handlers**:
- `CreateCategoryHandler` - Create custom categories
- `UpdateCategoryHandler` - Update category details
- `DeleteCategoryHandler` - Delete unused categories
- `ListCategoriesHandler` - List all categories

**Image Handlers**:
- `UploadImageHandler` - Upload and process menu item images

All handlers are registered in `Program.cs` DI container for injection into endpoints.

## 🔄 CI/CD Pipeline

The project uses **GitHub Actions** for continuous integration and deployment:

### Workflows

**1. CI Workflow** (`.github/workflows/ci.yml`)
- **Triggers**: Push to `main`, Pull Requests
- **Jobs**:
  - **Build and Test**: 
    - Runs on Ubuntu with PostgreSQL service container
    - Restores dependencies, builds solution (Release)
    - Runs unit tests and integration tests
    - Publishes test results with `dotnet-trx` reporter
  - **Code Quality**: 
    - Checks code formatting with `dotnet format`
    - Runs security scans for vulnerabilities
  - **Build Docker** (main branch only):
    - Builds Docker image with caching
    - Tags image with commit SHA

**2. Code Coverage Workflow** (`.github/workflows/coverage.yml`)
- **Triggers**: Push to `main`, Pull Requests
- **Features**:
  - Runs tests with XPlat Code Coverage collector
  - Generates HTML/Cobertura coverage reports using ReportGenerator
  - Uploads coverage artifacts
  - Adds coverage summary as PR comment
  - Enforces **70% coverage threshold** (fails if below)

**3. PR Validation Workflow** (`.github/workflows/pr-validation.yml`)
- **Triggers**: Pull Request opened/updated
- **Checks**:
  - **Semantic PR titles**: Enforces conventional commit format
    - Types: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`, `revert`
  - **Merge conflicts**: Auto-labels PRs with conflicts
  - **PR size labeling**: Auto-labels by changed lines (xs/s/m/l/xl)
  - **Security scan**: 
    - Checks for vulnerable NuGet packages
    - Scans for leaked secrets with TruffleHog

### Branch Protection Rules (Recommended)

Configure these settings in GitHub repository settings:

```yaml
Protect main branch:
  - Require pull request reviews (1 approver)
  - Require status checks to pass:
    - Build and Test
    - Code Quality
    - Test Coverage
    - Security Scan
  - Require branches to be up to date
  - Require conversation resolution before merging
  - Require linear history
  - Do not allow bypassing settings
```

### Running CI Locally

```bash
# Install act (GitHub Actions runner)
# https://github.com/nektos/act

# Run CI workflow locally
act -j build-and-test

# Run with specific event
act pull_request -j build-and-test
```

## 📝 Future Improvements / TODO

### Image Management Optimization
- **Orphan Image Cleanup Service**: Implement background job to detect and delete orphaned images from blob storage
  - Find images in blob storage not referenced in any menu (deleted items, abandoned drafts)
  - Delete images for menus deleted >3 days ago (grace period for restoration)
  - Delete orphaned item images older than 7 days
  - Can be implemented as:
    - Azure Function with timer trigger (daily)
    - Background service in the API using `IHostedService`
    - Azure Blob lifecycle management policies (automatic)
- **Alternative: Temporary Blob Container**
  - Upload images to temp container first
  - Move to permanent storage only when menu is saved
  - Auto-retention policy on temp container (3 days)
  - Requires image copy operation and URL updates

### Missing Features
- **Clone Menu Endpoint**: Copy existing menu to create variations (e.g., Summer 2025 → Summer 2026)
  - POST `/api/cafes/{cafeId}/menus/{menuId}/clone`
  - Request: `{ "newMenuName": "Summer 2026" }`
  - Copies entire menu structure (sections, items, categories, ingredients)
  - Generates new GUIDs for menu, sections, and items
  - Images can be shared or duplicated based on requirements
  - Publishes `MenuClonedEvent`

### Additional Features
- **Menu versioning**: Track menu changes over time
- **Bulk operations**: Import/export menus as JSON
- **Image optimization**: WebP conversion, multiple sizes for responsive design
- **Caching layer**: Redis cache for frequently accessed active menus
- **Analytics**: Track popular items, menu view counts
- **Search**: Full-text search across menu items

## 🔄 Domain Events

Events published to Azure Service Bus:

- `MenuCreatedEvent`
- `MenuUpdatedEvent`
- `MenuDeletedEvent`
- `MenuPublishedEvent`
- `MenuActivatedEvent`
- `MenuDeactivatedEvent`
- `MenuClonedEvent`

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is part of the SmartCafe system. All rights reserved.

## 📧 Contact

For questions or support, please contact the development team.

---

Built with ❤️ using .NET 10 and Clean Architecture
