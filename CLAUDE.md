# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build
dotnet build -c Release

# Run (Aspire orchestration — recommended for local dev)
cd src/SmartCafe.Menu.AppHost && dotnet run

# Tests
dotnet test                                          # all tests
dotnet test tests/SmartCafe.Menu.UnitTests          # unit tests only
dotnet test tests/SmartCafe.Menu.IntegrationTests   # integration tests only
dotnet test /p:CollectCoverage=true                 # with coverage

# Run a single test
dotnet test --filter "FullyQualifiedName~HandlerName"

# EF Core migrations
dotnet ef migrations add <Name> --project src/SmartCafe.Menu.Infrastructure --startup-project src/SmartCafe.Menu.API
dotnet ef database update --project src/SmartCafe.Menu.Infrastructure --startup-project src/SmartCafe.Menu.API
```

## Architecture

Clean Architecture with Vertical Slice feature organization. The solution is a .NET 10 microservice managing digital menus for cafes (multi-menu support, state transitions, image storage, and domain event publishing).

**Projects:**
- `SmartCafe.Menu.Domain` — Entities, value objects, domain events. No external dependencies.
- `SmartCafe.Menu.Application` — Use cases organized as vertical slices in `Features/`. Contains commands/queries, handlers, validators, mappers, and DTOs per feature. Uses a mediator pattern with `ValidationBehavior`.
- `SmartCafe.Menu.Infrastructure` — EF Core + PostgreSQL, Azure Blob Storage, Azure Service Bus, ImageSharp thumbnail generation.
- `SmartCafe.Menu.API` — Minimal API endpoints (no controllers). Thin layer that forwards to mediator.
- `SmartCafe.Menu.AppHost` — .NET Aspire orchestration. Starts PostgreSQL, Azurite, pgAdmin, and the API together.
- `SmartCafe.Menu.Migrator` — Standalone CLI tool for running EF migrations (used by AppHost before API startup).
- `SmartCafe.Menu.Shared` — Shared `Result<T>`, `Error`, `ErrorType` models and provider interfaces.

**Test Projects:**
- `UnitTests` — xUnit + NSubstitute. Tests handlers and validators in isolation.
- `IntegrationTests` — Testcontainers (real PostgreSQL) + `WebApplicationFactory`. Tests full API stack.
- `SmartCafe.Menu.Tests.Shared` — Bogus data generators and shared mock implementations.

## Key Patterns

**Result pattern — no exceptions for expected errors.** All handlers return `Result<T>`. Errors are `Error.NotFound(...)`, `Error.Conflict(...)`, or `Error.Validation(...)`. The API layer maps these via `ResultExtensions` (`ToApiResult`, `ToCreatedResult`, `ToNoContentResult`). Never throw exceptions for business logic failures.

**Two-tier validation:**
1. FluentValidation validators catch format issues → 400 responses before the handler runs.
2. Handlers perform business logic checks (entity existence, state transitions) → 404/409 responses.
Validation messages live in `ValidationMessages` constants; error codes in `ErrorCodes` constants.

**Menu state machine:** `New → Published → Active`. Only one active menu per cafe at a time (enforced by a unique partial index). State transitions are explicit commands (`PublishMenu`, `ActivateMenu`).

**Entity IDs:** Use `Guid.CreateVersion7()` for all new entity IDs (time-ordered for better index performance).

**Time:** Always inject `IDateTimeProvider` rather than calling `DateTime.UtcNow` directly.

**Manual mappers:** Each feature has a `Mappers/` subfolder with static classes. No AutoMapper or similar library.

**Endpoint registration:** Each endpoint is a static extension method on `RouteGroupBuilder` in `src/SmartCafe.Menu.API/Endpoints/`. Registration is wired up in `WebApplicationExtensions`.

## Code Style

- `TreatWarningsAsErrors = true` — all warnings break the build.
- File-scoped namespaces, `var` everywhere, 4-space indents.
- `using` directives must be ordered (System → external) and unused ones cause build errors (`IDE0005 = error`).
- Trailing spaces and missing final newlines are enforced by `.editorconfig`.
