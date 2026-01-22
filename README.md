# SmartCafe Menu Service

[![CI](https://github.com/petro-konopelko/smartcafe-menu/actions/workflows/ci.yaml/badge.svg)](https://github.com/petro-konopelko/smartcafe-menu/actions/workflows/ci.yaml)
[![PR](https://github.com/petro-konopelko/smartcafe-menu/actions/workflows/pr.yaml/badge.svg)](https://github.com/petro-konopelko/smartcafe-menu/actions/workflows/pr.yaml)

A .NET 10 microservice for managing digital menus in the SmartCafe smart ordering system. Built with Clean Architecture, PostgreSQL, and Azure services.

## 🏗️ Architecture

- **Clean Architecture** with Vertical Slice Architecture
- **Result Pattern**: Zero exception-based error handling in application layer
- **Domain Layer**: Entities, value objects, domain events
- **Application Layer**: Handlers (return `Result<T>`), DTOs, validators, manual per-feature mappers
- **Infrastructure Layer**: EF Core, PostgreSQL, Azure Blob Storage, Azure Service Bus
- **API Layer**: ASP.NET Core Minimal API with Result extensions

## 🚀 Features

- **Multi-Menu Management**: Cafes can have multiple menus (e.g., Summer, Winter, Holiday)
- **Menu States**: New → Published → Active (only one active menu per cafe)
- **Menu Activation**: Switch active menus seamlessly
- **Menu Cloning**: Create variations from existing menus
- **Section Management**: Organize items by meal type with availability hours
- **Item Management**: Full CRUD with images and ingredients
- **Image Processing**: Auto-generate cropped thumbnails
- **Event Publishing**: Publish domain events to Azure Service Bus
- **Time-Ordered GUIDs**: Use `Guid.CreateVersion7()` for better database performance

## 🛠️ Tech Stack

- **.NET 10** with C# 14 (latest language features)
- **ASP.NET Core Minimal API**
- **Entity Framework Core 10** with PostgreSQL
- **FluentValidation** for input validation
- Manual mapping via feature-local static mappers
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
│   │   ├── Features/*/Mappers/          # Manual static mappers per feature
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
    public async Task<Result<CreateMenuResponse>> HandleAsync(CreateMenuRequest request, CancellationToken ct)
    {
        // Existence check → 404 Not Found
        if (menu == null)
            return Result<CreateMenuResponse>.Failure(Error.NotFound(
                "Menu not found", ErrorCodes.MenuNotFound));
        
        // Return success with value
        return Result<CreateMenuResponse>.Success(new CreateMenuResponse(...));
    }
}
```

**Endpoints use Result extensions:**
```csharp
group.MapPost("/", async (Guid cafeId, CreateMenuRequest request, IMediator mediator, CancellationToken ct) => 
{
    var command = request with { CafeId = cafeId };
    var result = await mediator.Send<CreateMenuRequest, Result<CreateMenuResponse>>(command, ct);
    // Factory delegate ensures safe access to response.Id only on success
    return result.ToCreatedResult(response => $"/api/cafes/{cafeId}/menus/{response.Id}");
})
.WithName("CreateMenu")
.WithSummary("Create a new menu in New state");
```

**Error Types:**
- `Error.NotFound()` → 404 Not Found
- `Error.Validation()` → 400 Bad Request
- `Error.Conflict()` → 409 Conflict

### Validation Architecture

The project uses a **two-tier validation strategy** for clean separation of concerns:

**1. Format Validation (FluentValidation → 400 Bad Request)**
- Handled by `AbstractValidator<T>` classes
- Validates: required fields, string length, format, range
- Uses centralized `ValidationMessages` constants
- Executed automatically by `ValidationBehavior<TRequest, T>` before handler execution
- Returns `Result<T>.Failure(Error.Validation(...))` with all validation errors

**2. Existence & Business Rule Validation (Handlers → 404/409)**
- Handled directly in command/query handlers
- Validates: entity existence, business rules, state transitions
- Uses centralized `ErrorCodes` constants
- Returns `Result<T>.Failure(Error.NotFound/Conflict(...))`

**Example - ValidationMessages:**
```csharp
public static class ValidationMessages
{
    public const string CafeIdRequired = "Cafe ID is required.";
    public const string MenuNameRequired = "Menu name is required.";
    public const string MenuNameMaxLength = "Menu name must not exceed 200 characters.";
    // ... 20+ constants
}

public class CreateMenuValidator : AbstractValidator<CreateMenuRequest>
{
    public CreateMenuValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.MenuNameRequired)
            .MaximumLength(200).WithMessage(ValidationMessages.MenuNameMaxLength);
    }
}
```

**Example - ErrorCodes:**
```csharp
public static class ErrorCodes
{
    public const string CafeNotFound = "CAFE_NOT_FOUND";
    public const string MenuNotFound = "MENU_NOT_FOUND";
    public const string MenuAlreadyActive = "MENU_ALREADY_ACTIVE";
    // ... 10+ constants
}

public class ActivateMenuHandler
{
    public async Task<Result> HandleAsync(ActivateMenuCommand command, CancellationToken ct)
    {
        // Existence check → 404
        if (menu == null)
            return Result.Failure(Error.NotFound(
                "Menu not found", ErrorCodes.MenuNotFound));
        
        // Business rule → 409
        if (menu.IsActive)
            return Result.Failure(Error.Conflict(
                "Menu is already active", ErrorCodes.MenuAlreadyActive));
    }
}
```

**Benefits:**
- Clear separation: format vs business logic
- Centralized messages → easy to update
- No string duplication across validators/handlers
- Testable: mock validators or handlers independently
- Consistent error responses

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
- `Menus` - Menu definitions with state (New/Published/Active)
- `Sections` - Menu sections (Breakfast, Lunch, etc.)
- `MenuItems` - Individual menu items

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

### Cafe Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/cafes` | List all active cafes |
| POST | `/api/cafes` | Create new cafe |
| GET | `/api/cafes/{cafeId}` | Get cafe details |
| DELETE | `/api/cafes/{cafeId}` | Soft delete cafe |

**Note:** All menu operations return 404 when the cafe is soft deleted.

### Menu Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/cafes/{cafeId}/menus` | List all menus |
| POST | `/api/cafes/{cafeId}/menus` | Create new menu (new) |
| GET | `/api/cafes/{cafeId}/menus/{menuId}` | Get menu details |
| PUT | `/api/cafes/{cafeId}/menus/{menuId}` | Update menu |
| DELETE | `/api/cafes/{cafeId}/menus/{menuId}` | Delete menu (new only) |
| POST | `/api/cafes/{cafeId}/menus/{menuId}/publish` | Publish menu |
| POST | `/api/cafes/{cafeId}/menus/{menuId}/activate` | Activate menu |
| POST | `/api/cafes/{cafeId}/menus/{menuId}/clone` | Clone menu |
| GET | `/api/cafes/{cafeId}/menus/active` | Get active menu (public) |

See [Swagger UI](http://localhost:5000/swagger) for complete API documentation.

## 🏛️ Clean Architecture Implementation

### Handler Pattern
All endpoints follow the **Handler Pattern** with **Result Pattern** for separation of concerns:

**Endpoint Responsibilities** (thin, 20-50 lines):
- Receive HTTP requests and extract parameters
- Call mediator to execute handler
- Map Result<T> to HTTP responses using extension methods
- NO try-catch blocks (Result pattern handles errors)

**Handler Responsibilities** (business logic):
- Execute business logic
- Coordinate repositories and services
- Validate existence and business rules
- Publish domain events
- Return Result<T> (never throw exceptions for business errors)

**Example**:
```csharp
// Endpoint (thin layer)
public static RouteGroupBuilder MapPublishMenu(this RouteGroupBuilder group)
{
    group.MapPost("/{menuId:guid}/publish", async (
        Guid cafeId,
        Guid menuId,
        PublishMenuCommand command,
        IMediator mediator,
        CancellationToken ct) =>
    {
        var publishCommand = new PublishMenuCommand(cafeId, menuId);
        var result = await mediator.Send<PublishMenuCommand, Result>(publishCommand, ct);
        return result.ToNoContentResult(); // Auto-maps errors to HTTP status
    })
    .WithName("PublishMenu")
    .WithSummary("Publish a new menu to make it ready for activation");
    
    return group;
}

// Handler (business logic)
public class PublishMenuHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<PublishMenuCommand, Result>
{
    public async Task<Result> HandleAsync(
        PublishMenuCommand command, CancellationToken cancellationToken)
    {
        var menu = await menuRepository.GetByIdAsync(command.MenuId, cancellationToken);
        
        // Existence check → 404 Not Found
        if (menu == null || menu.CafeId != command.CafeId)
            return Result.Failure(Error.NotFound(
                "Menu not found", ErrorCodes.MenuNotFound));
        
        // Business rule checks → 409 Conflict
        if (menu.IsPublished)
            return Result.Failure(Error.Conflict(
                "Menu is already published", ErrorCodes.MenuAlreadyPublished));
        
        if (menu.Sections.Count == 0)
            return Result.Failure(Error.Conflict(
                "Menu must have at least one section", ErrorCodes.MenuHasNoSections));
        
        // Business logic
        menu.IsPublished = true;
        menu.PublishedAt = dateTimeProvider.UtcNow;
        
        await menuRepository.UpdateAsync(menu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        // Publish domain event
        await eventPublisher.PublishAsync(
            new MenuPublishedEvent(Guid.CreateVersion7(), menu.Id, menu.CafeId, menu.Name, dateTimeProvider.UtcNow),
            cancellationToken);
        
        return Result.Success();
    }
}
```

### Implemented Handlers

All handlers follow the **Result Pattern** - returning `Result<T>` instead of throwing exceptions.

### Manual Mapping (no Mapperly)
- Mapping from domain entities to response DTOs is implemented via small, feature-local static mapper classes under each handler folder (e.g., `Features/Menus/CreateMenu/Mappers/CreateMenuMapper.cs`).
- Handlers call these mappers for response construction instead of inlining DTO creation.
- Shared DTOs live under `Features/Menus/Shared`, but mapping stays close to the feature for clarity and maintainability.

**Menu Handlers** (Application/Features/Menus/):
- `CreateMenuHandler` - Create new menu in New state
- `UpdateMenuHandler` - Update existing menu structure
- `DeleteMenuHandler` - Delete New menus with blob cleanup
- `GetMenuHandler` - Retrieve menu details by ID
- `GetActiveMenuHandler` - Get currently active menu for customers
- `ListMenusHandler` - List all menus for a cafe with pagination
- `ActivateMenuHandler` - Activate a published menu (deactivates previous)
- `PublishMenuHandler` - Publish a new menu (ready for activation)
- `CloneMenuHandler` - Clone existing menu to create variations

**Image Handlers** (Application/Features/Images/):
- `UploadImageHandler` - Upload and process menu item images to Azure Blob Storage

All handlers:
- Return `Result<T>` or `Result` (never throw business exceptions)
- Use `ErrorCodes` constants for consistent error codes
- Use `IDateTimeProvider` for testable timestamps
- Publish domain events via `IEventPublisher`
- Are registered in DI container via `ApplicationServiceRegistration`

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
  - Find images in blob storage not referenced in any menu (deleted items, abandoned new menus)
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
