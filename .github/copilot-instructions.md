# Menu Service - Copilot Instructions

---
applyTo: "**"
---

> **Note:** This file should be placed in the root of the Menu Service implementation repository as `.github/copilot-instructions.md` to guide GitHub Copilot during development.

## Project Overview

This is the **Menu Service** for SmartCafe, a microservices-based smart ordering system. The Menu Service manages digital menus for cafes, including sections, items, categories, and images.

**Repository Context:**
- This is a standalone microservice within the SmartCafe ecosystem
- Designed to be deployed independently on Azure
- Communicates with other services via Azure Service Bus events
- Provides REST API for frontend applications and other services

## Architecture

- **Pattern:** Clean Architecture with Vertical Slice Architecture
- **Framework:** .NET 10 Web API with Minimal API
- **Database:** PostgreSQL with Entity Framework Core 10
- **Cloud Services:** Azure Blob Storage, Azure Service Bus, Azure Key Vault
- **Patterns:** Repository, Unit of Work, Domain Events
- **Key Features:** Time-ordered GUIDs (CreateVersion7), IDateTimeProvider for testability, endpoint filters

## Project Structure

```
src/
├── SmartCafe.Menu.AppHost/              # .NET Aspire orchestration
├── SmartCafe.Menu.ServiceDefaults/      # Shared Aspire configuration
├── SmartCafe.Menu.Migrator/             # Database migration tool for local development
├── SmartCafe.Menu.Domain/               # Core business logic and entities
│   ├── Entities/                        # Menu, Section, MenuItem, Category, etc.
│   ├── Events/                          # Domain events (MenuCreated, MenuActivated, etc.)
│   ├── Exceptions/                      # Domain exceptions (MenuNotFoundException)
│   ├── Interfaces/                      # IDateTimeProvider
│   ├── Services/                        # Domain services
│   └── ValueObjects/                    # Ingredient, AvailabilityHours
├── SmartCafe.Menu.Application/          # Use cases, DTOs, interfaces, validators
│   ├── Common/Results/                  # Result pattern (Result<T>, Error, ErrorType, ErrorDetail, None)
│   ├── Common/Validators/               # ValidationMessages constants for FluentValidation
│   ├── Features/                        # Vertical slices by feature
│   │   ├── Menus/                       # Menu feature handlers
│   │   │   ├── ActivateMenu/            # ActivateMenuCommand, ActivateMenuHandler, ActivateMenuCommandValidator
│   │   │   ├── CloneMenu/               # CloneMenuRequest, CloneMenuHandler, CloneMenuRequestValidator
│   │   │   ├── CreateMenu/              # CreateMenuRequest, CreateMenuHandler, CreateMenuRequestValidator
│   │   │   ├── DeleteMenu/              # DeleteMenuCommand, DeleteMenuHandler, DeleteMenuCommandValidator
│   │   │   ├── GetActiveMenu/           # GetActiveMenuQuery, GetActiveMenuHandler, GetActiveMenuQueryValidator
│   │   │   ├── GetMenu/                 # GetMenuQuery, GetMenuHandler, GetMenuQueryValidator
│   │   │   ├── ListMenus/               # ListMenusQuery, ListMenusHandler, ListMenusQueryValidator
│   │   │   ├── PublishMenu/             # PublishMenuCommand, PublishMenuHandler, PublishMenuCommandValidator
│   │   │   ├── UpdateMenu/              # UpdateMenuRequest, UpdateMenuHandler, UpdateMenuRequestValidator
│   │   │   └── Shared/                  # Shared DTOs (MenuSectionDto, MenuItemDto, etc.) with validators
│   │   └── Images/                      # Image feature handlers
│   │       ├── UploadImage/             # UploadImageCommand, UploadImageHandler
│   │       └── Models/                  # Shared image models
│   ├── Interfaces/                      # Repository interfaces (ICafeRepository, IMenuRepository, ICategoryRepository)
│   ├── Mappings/                        # Mapperly mapper classes
│   ├── Mediation/                       # Mediator pattern implementation
│   │   ├── Core/                        # IMediator, IHandler interfaces
│   │   └── Behaviors/                   # ValidationBehavior (wraps FluentValidation)
│   └── DependencyInjection/             # Application layer service registration
├── SmartCafe.Menu.Infrastructure/       # Data access (EF Core), Azure services
│   ├── Data/                            # EF Core DbContext, configurations
│   ├── Repositories/                    # Repository implementations (CafeRepository, MenuRepository, CategoryRepository)
│   ├── EventBus/                        # Azure Service Bus publisher
│   ├── BlobStorage/                     # Azure Blob Storage service
│   ├── Services/                        # DateTimeProvider, ImageProcessingService
│   ├── Migrations/                      # EF Core migrations
│   └── DependencyInjection/             # Infrastructure layer service registration
└── SmartCafe.Menu.API/                  # Minimal API endpoints, filters, middleware
    ├── Endpoints/                       # Vertical slices by feature
    │   ├── Menus/                       # Menu endpoints
    │   │   ├── ActivateMenuEndpoint.cs  # POST /menus/{menuId}/activate
    │   │   ├── CloneMenuEndpoint.cs     # POST /menus/{menuId}/clone
    │   │   ├── CreateMenuEndpoint.cs    # POST /menus
    │   │   ├── DeleteMenuEndpoint.cs    # DELETE /menus/{menuId}
    │   │   ├── GetActiveMenuEndpoint.cs # GET /menus/active
    │   │   ├── GetMenuEndpoint.cs       # GET /menus/{menuId}
    │   │   ├── ListMenusEndpoint.cs     # GET /menus
    │   │   ├── MenuRoutes.cs            # Centralized route URL generation
    │   │   ├── PublishMenuEndpoint.cs   # POST /menus/{menuId}/publish
    │   │   └── UpdateMenuEndpoint.cs    # PUT /menus/{menuId}
    │   └── Images/                      # Image endpoints
    │       └── UploadImageEndpoint.cs   # POST /menus/{menuId}/items/{itemId}/image
    ├── Extensions/                      # Extension methods
    │   ├── ResultExtensions.cs          # Result → HTTP mapping (ToApiResult, ToCreatedResult)
    │   └── WebApplicationExtensions.cs  # Endpoint registration helpers
    ├── Filters/                         # Endpoint filters
    │   └── ValidationFilter.cs          # Validation filter (FluentValidation)
    ├── Middleware/                      # Middleware
    │   └── ExceptionHandlingMiddleware.cs # Global exception handler (unexpected errors)
    └── Program.cs                       # Application startup

tests/
├── SmartCafe.Menu.UnitTests/
│   ├── Application/                     # Handler tests
│   ├── Domain/                          # Entity tests
│   └── Fakes/                           # FakeDateTimeProvider, etc.
└── SmartCafe.Menu.IntegrationTests/
    ├── Api/                             # API integration tests
    ├── Endpoints/                       # Endpoint tests
    └── Fixtures/                        # Test fixtures (WebApplicationFactory)
```

## Code Style & Guidelines

> **Important:** These guidelines apply to the Menu Service implementation code, not documentation.

### General Principles
- Follow SOLID principles
- Use Clean Architecture dependency rules (Domain → Application → Infrastructure)
- Use Minimal API with endpoint filters for cross-cutting concerns
- Use async/await consistently
- **Use IDateTimeProvider** instead of DateTime.UtcNow for testability
- All DateTime values must be in UTC
- Implement comprehensive logging with Serilog
- The Serilog should use Two-stage initialization
- Always include correlation IDs for request tracking
- Use C# 14 features (primary constructors, collection expressions, etc.)
- Treat warnings as errors
- Language version should be latest

### Naming Conventions
- Use PascalCase for classes, methods, properties
- Use camelCase for parameters and local variables
- Prefix interfaces with 'I' (e.g., `IMenuRepository`, `IDateTimeProvider`)
- Suffix DTOs with purpose (e.g., `CreateMenuRequest`, `MenuResponse`)
- Use descriptive names that reflect business domain
- Entity IDs are Guid type using `Guid.CreateVersion7()` for time-ordered UUIDs
- Endpoint files: `{Feature}Endpoint.cs` (e.g., `CreateMenuEndpoint.cs`)

### Domain Layer
- Entities should be rich domain models with behavior
- Use `Guid.CreateVersion7()` for all entity IDs (time-ordered UUIDs)
- Use value objects for complex types (e.g., `AvailabilityHours`, `Ingredient`)
- Raise domain events for significant state changes
- Keep domain logic independent of infrastructure concerns
- Validate business rules in entity methods
- Never use `DateTime.UtcNow` directly - inject `IDateTimeProvider`
- All DateTime properties should be UTC

### Application Layer
- Define request/response DTOs for each endpoint
- **Validation Strategy**: Clear separation between format validation and existence checking
  - **Validators (FluentValidation)**: Format validation only (NotEmpty, MaxLength, etc.) → Returns 400 Bad Request
  - **Handlers**: Existence checks and business logic → Returns 404 Not Found or 409 Conflict
- **Validation Messages**: Use constants from `Application/Common/Validators/ValidationMessages.cs`
  - Example: `.NotEmpty().WithMessage(ValidationMessages.CafeIdRequired)`
  - Centralized messages for consistency and maintainability
- **Error Codes**: Use constants from `Domain/ErrorCodes.cs`
  - Example: `Error.NotFound(message, ErrorCodes.MenuNotFound)`
  - All handlers use error code constants, never string literals
- Validators are simple, synchronous, and have NO database dependencies
- Existence validation happens in handlers using repository calls
- Map entities to DTOs using Mapperly (compile-time source generator)
- Keep business logic in domain or application services
- Interfaces define contracts (repositories, services)
- Use vertical slice organization (group by feature, not layer)
- **All handlers return `Result<T>` or `Result`** - never throw exceptions for business errors
- Handler interface: `ICommandHandler<TRequest, Result<TResponse>>` or `IQueryHandler<TRequest, Result<TResponse>>`
- Use `Result.Success()` for void operations, `Result<T>.Success(value)` for value returns
- Use `Result.Failure(Error.NotFound/Validation/Conflict(...))` for errors

### Infrastructure Layer
- Implement repositories using Entity Framework Core with PostgreSQL
- Use Npgsql provider for PostgreSQL
- Configure all DateTime columns as `timestamp with time zone`
- Use JSONB column type for flexible data (e.g., ingredient options)
- Implement `IDateTimeProvider` with production and fake implementations
- Use Unit of Work pattern for transaction management
- Store images in Azure Blob Storage with consistent naming
- Publish events to Azure Service Bus
- Use dependency injection for all services
- Retrieve secrets from Azure Key Vault or environment variables (never from appsettings.json)

### API Layer (Minimal API)
- Use Minimal API endpoints with route groups for organization
- Implement endpoint filters for validation and logging
- RESTful API design with route constraints (e.g., `{cafeId:guid}`)
- Use standard HTTP status codes
- Return DTOs, never domain entities
- **Use Result extension methods** (`ToApiResult()`, `ToCreatedResult()`, `ToNoContentResult()`)
- **Use ProblemDetails for all errors** - add `.ProducesValidationProblem()` and `.ProducesProblem(statusCode)` to all endpoints
- **Add `.WithName("OperationName")` and `.WithSummary("Description")` to all endpoints** for OpenAPI documentation
- **Do NOT use `.WithOpenApi()`** - it's deprecated in .NET 10
- Enable CORS for frontend origins
- **No try-catch blocks** - Result pattern handles errors
- Endpoint pattern:
  ```csharp
  public static RouteGroupBuilder MapCreateMenu(this RouteGroupBuilder group)
  {
      group.MapPost("/", async (Guid cafeId, CreateMenuRequest request, IMediator mediator, CancellationToken ct) => 
      {
          var command = request with { CafeId = cafeId };
          var result = await mediator.Send<CreateMenuRequest, Result<CreateMenuResponse>>(command, ct);
          return result.ToCreatedResult(response => MenuRoutes.GetMenuLocation(cafeId, response.Id));
      })
      .WithName("CreateMenu")
      .WithSummary("Create a new menu with sections and items")
      .Produces<CreateMenuResponse>(StatusCodes.Status201Created)
      .ProducesValidationProblem()
      .ProducesProblem(StatusCodes.Status404NotFound);
      return group;
  }
  ```

## Business Rules

### Menus
- A cafe can have **multiple menus** (e.g., Summer Menu, Winter Menu, Holiday Special)
- Only **one menu can be active** at a time (enforced by unique partial index)
- Menu states: Draft → Published → Active
- **Draft**: Work in progress, not visible to customers, can be edited/deleted
- **Published**: Ready for activation, not currently active, cannot be deleted
- **Active**: Currently displayed to customers, changes go live immediately
- Must publish menu before it can be activated
- Activating a menu automatically deactivates the previous active menu
- Menus can be cloned to create variations (e.g., Summer 2025 → Summer 2026)
- Each menu identified by time-ordered GUID (Guid.CreateVersion7)

### Sections
- Sections organize menu items by meal type or theme (e.g., Breakfast, Lunch, Dinner)
- Each section has optional availability hours (TimeSpan for from/to)
- Maximum 100 items per section
- Section names must be unique within a menu
- Sections have display order for UI rendering (drag-and-drop reordering)
- Sections are embedded within menus (saved atomically with menu)

### Menu Items
- Items belong to one section
- Items can have multiple categories (many-to-many relationship)
- Items must have at least one category assigned
- Price must be positive (enforced by CHECK constraint)
- Images are optional; use defaults if none provided
- Support two image types: big (main display) and cropped (thumbnail/fast scrolling)
- Items can be soft-deleted (IsActive flag)
- Ingredient options stored as JSONB in PostgreSQL
- Each item identified by time-ordered GUID

### Categories
- Two default categories: Vegetarian, Spicy
- Cafes can create custom categories
- Categories can have optional icons/images
- Categories are reusable across all cafes

### Images
- Store in Azure Blob Storage
- Generate cropped version automatically on upload
- Naming: `{itemId}_big.{ext}` and `{itemId}_cropped.{ext}`
- Supported formats: JPEG, PNG, WebP
- Maximum file size: 5MB
- Delete from blob storage when item/menu is deleted

### Ingredients
- Items can have configurable ingredient options
- Customers can include or exclude ingredients when ordering
- Ingredient customization data is passed to Order Service

## Event Publishing

Publish domain events to Azure Service Bus for integration:

- `MenuCreatedEvent` - When a menu is created (draft)
- `MenuUpdatedEvent` - When menu details or structure changes
- `MenuDeletedEvent` - When a menu is soft-deleted
- `MenuPublishedEvent` - When a menu is published (draft → published)
- `MenuActivatedEvent` - When a menu is activated (becomes active for customers)
- `MenuDeactivatedEvent` - When a menu is deactivated (replaced by another)
- `MenuClonedEvent` - When a menu is cloned to create a new menu

Events should include: 
- eventId (Guid.CreateVersion7)
- menuId (Guid)
- cafeId (Guid)
- timestamp (UTC via IDateTimeProvider)
- eventType (string)
- relevant payload (menu name, activation details, etc.)

**Important:** Use `IDateTimeProvider.UtcNow` for all event timestamps.

## Testing Guidelines

- Write unit tests for domain logic and application services
- Use xUnit as the test framework
- Mock `IDateTimeProvider` to freeze time in tests (use `FakeDateTimeProvider`)
- Use Testcontainers for PostgreSQL in integration tests
- Test Minimal API endpoints using `WebApplicationFactory`
- Test endpoint filters (validation, logging)
- Mock external dependencies (blob storage, service bus, Key Vault)
- Aim for high code coverage on business logic
- Test validation rules thoroughly (FluentValidation)
- Test domain events are raised correctly
- Test unique constraints (one active menu per cafe)
- Test menu activation workflow (deactivates previous active menu)
- Test Result pattern success and failure paths

## Error Handling - Result Pattern

**This project uses the Result pattern to eliminate exception-based error handling in the application layer.**

### Result Pattern Structure
- `Result<T>` (non-sealed base class) - Generic result with value
- `Result` (sealed class inheriting `Result<None>`) - Result for void operations
- `None` (sealed singleton) - Represents "no return value" (use `None.Instance`)
- `Error` - Error envelope containing `ErrorType` and list of `ErrorDetail`
- `ErrorType` enum - Application-level error types: `NotFound`, `Validation`, `Conflict`
- `ErrorDetail` record - Individual error: `Message`, `Code?`, `Field?`

### Handler Implementation
```csharp
public class CreateMenuHandler : ICommandHandler<CreateMenuRequest, Result<CreateMenuResponse>>
{
    public async Task<Result<CreateMenuResponse>> HandleAsync(CreateMenuRequest request, CancellationToken ct)
    {
        // Check cafe exists (404)
        var cafeExists = await cafeRepository.ExistsAsync(request.CafeId, ct);
        if (!cafeExists)
            return Result<CreateMenuResponse>.Failure(Error.NotFound(
                $"Cafe with ID {request.CafeId} not found", 
                ErrorCodes.CafeNotFound));
        
        // Validation error (400 - multiple details supported)
        if (missingCategories.Any())
            return Result<CreateMenuResponse>.Failure(Error.Validation(
                new ErrorDetail("Categories not found", ErrorCodes.CategoriesNotFound)));
        
        // Conflict error (409)
        if (menu.IsActive)
            return Result<CreateMenuResponse>.Failure(Error.Conflict(
                "Menu is already active", 
                ErrorCodes.MenuAlreadyActive));
        
        // Success
        return Result<CreateMenuResponse>.Success(new CreateMenuResponse(...));
    }
}
```

### Endpoint Implementation
```csharp
public static RouteGroupBuilder MapCreateMenu(this RouteGroupBuilder group)
{
    group.MapPost("/", async (Guid cafeId, CreateMenuRequest request, IMediator mediator, CancellationToken ct) =>
    {
        var command = request with { CafeId = cafeId };
        var result = await mediator.Send<CreateMenuRequest, Result<CreateMenuResponse>>(command, ct);
        return result.ToCreatedResult(response => $"/api/cafes/{cafeId}/menus/{response.Id}");
    })
    .WithName("CreateMenu")
    .WithSummary("Create a new menu with sections and items")
    .Produces<CreateMenuResponse>(StatusCodes.Status201Created)
    .ProducesValidationProblem()
    .ProducesProblem(StatusCodes.Status404NotFound);
    return group;
}
```

### Result Extension Methods (API Layer)
- `result.ToApiResult()` → Maps to `Ok(200)` or error status
- `result.ToCreatedResult(response => location)` → Maps to `Created(201, location)` or error status (uses factory delegate for safe access)
- `result.ToNoContentResult()` → Maps to `NoContent(204)` or error status

### Error Status Mapping
- `ErrorType.NotFound` → `404 Not Found`
- `ErrorType.Validation` → `400 Bad Request`
- `ErrorType.Conflict` → `409 Conflict`
- Unknown types → throws exception (fail-fast)

### ValidationBehavior Integration
- `ValidationBehavior<TRequest, T>` automatically wraps FluentValidation errors
- Returns `Result<T>.Failure(Error.Validation(...))` with all validation errors
- Zero reflection - works with `Result<T>` constraint

### Guidelines
- **NEVER throw exceptions** in application layer handlers for business errors
- Return `Result.Failure()` with appropriate `ErrorType` instead
- Use domain exceptions only for truly exceptional cases (not found in handlers)
- Middleware handles unexpected exceptions (500 Internal Server Error)
- Log all errors with context
- Never expose stack traces in production

## Security & Access Control

- **Secrets Management**:
  - Never store passwords/connection strings in appsettings.json
  - Use Azure Key Vault for production secrets (with Managed Identity)
  - Use User Secrets for local development
  - Use environment variables as fallback
  - Build connection strings dynamically from config + secrets
- Use GUID identifiers to prevent enumeration attacks (no sequential integers)
- Validate cafe ownership before modifications (future enhancement)
- Sanitize user input to prevent injection attacks
- Use HTTPS only in production
- Enable SSL/TLS for PostgreSQL connections
- Implement role-based access control (future enhancement)
- Principle of least privilege for database user permissions

## Performance Considerations

- Cache published menus (consider Redis)
- Use pagination for list endpoints
- Optimize database queries with proper indexes
- Use async I/O for all external calls
- Implement request/response compression
- Consider CDN for serving menu images

## Deployment

- Docker support with multi-stage builds
- Health checks for database and service bus
- Configuration via environment variables
- Support for multiple environments (Dev, Staging, Prod)
- CI/CD ready with GitHub Actions

## Documentation

- Keep OpenAPI/Swagger documentation up to date
- Document business rules in code comments
- Maintain architecture decision records (ADRs) in `/docs/adr/` folder
- Use clear, descriptive commit messages following conventional commits
- Update README.md with setup instructions

## How to Use This File

When you start implementing the Menu Service:

1. **Copy this file** to the Menu Service repository root as `.github/copilot-instructions.md`
2. GitHub Copilot will automatically use these instructions to provide context-aware suggestions
3. Update this file as architectural decisions evolve
4. Keep it synchronized with actual implementation patterns

## Example Project Structure

```
smartcafe-menu-service/
├── .github/
│   └── copilot-instructions.md  ← This file
├── src/
│   ├── SmartCafe.Menu.AppHost/
│   ├── SmartCafe.Menu.ServiceDefaults/
│   ├── SmartCafe.Menu.Domain/
│   ├── SmartCafe.Menu.Application/
│   ├── SmartCafe.Menu.Infrastructure/
│   └── SmartCafe.Menu.API/
├── tests/
│   ├── SmartCafe.Menu.UnitTests/
│   └── SmartCafe.Menu.IntegrationTests/
├── docs/
│   └── adr/  ← Architecture Decision Records
├── README.md
├── .gitignore
└── SmartCafe.Menu.sln
```

## Dependencies

**Key NuGet Packages:**
- **EF Core & Database:**
  - Microsoft.EntityFrameworkCore (10.0+)
  - Npgsql.EntityFrameworkCore.PostgreSQL
- **Validation & Mapping:**
  - FluentValidation
  - Mapperly (compile-time source generator)
- **Azure Services:**
  - Azure.Storage.Blobs
  - Azure.Messaging.ServiceBus
  - Azure.Security.KeyVault.Secrets
  - Azure.Identity (for Managed Identity)
- **Image Processing:**
  - SixLabors.ImageSharp
- **Logging & Observability:**
  - Serilog
  - OpenTelemetry (.NET, ASP.NET Core, Npgsql instrumentation)
- **API Documentation:**
  - Microsoft.AspNetCore.OpenApi
- **Development:**
  - .NET Aspire (Aspire.Hosting.PostgreSQL, Aspire.Hosting.Azure.*)
- **Testing:**
  - xUnit
  - Testcontainers.PostgreSQL
  - Microsoft.AspNetCore.Mvc.Testing (WebApplicationFactory)

## GitHub Actions
- Use GitHub Actions for CI/CD pipelines
- always use yaml files for defining workflows with .yaml extension

