# Validation Architecture

## Overview

The Menu Service implements a clear separation between input validation and business logic validation to ensure proper HTTP status code responses.

## Architecture Principles

### 1. Validators → Format Validation (400 Bad Request)
- **Purpose**: Validate input format and structure
- **Technology**: FluentValidation with `ValidationBehavior<TRequest, T>` pipeline
- **Returns**: `Error.Validation` → maps to `400 Bad Request`
- **Examples**:
  - `NotEmpty()` for required GUIDs
  - `MaximumLength()` for string fields
  - Complex object structure validation

### 2. Handlers → Existence & Business Logic (404/409)
- **Purpose**: Validate resource existence and business rules
- **Location**: Inside handler `HandleAsync` method
- **Returns**: 
  - `Error.NotFound` → maps to `404 Not Found`
  - `Error.Conflict` → maps to `409 Conflict`
- **Examples**:
  - Cafe exists check
  - Menu exists and belongs to cafe
  - Menu state transitions (Draft → Published → Active)

## Error Codes

Centralized in `Domain/ErrorCodes.cs`:

```csharp
// Resource Not Found (404)
ErrorCodes.CafeNotFound
ErrorCodes.MenuNotFound
ErrorCodes.CategoryNotFound

// Validation (400)
ErrorCodes.CategoriesNotFound

// Conflict (409)
ErrorCodes.MenuAlreadyActive
ErrorCodes.MenuAlreadyPublished
ErrorCodes.MenuNotPublished
ErrorCodes.CannotDeletePublishedMenu
```

## Validation Messages

Centralized in `Application/Common/Validators/ValidationMessages.cs`:

```csharp
// Common ID Validation
ValidationMessages.CafeIdRequired
ValidationMessages.MenuIdRequired
ValidationMessages.SourceMenuIdRequired

// Menu Validation
ValidationMessages.MenuNameRequired
ValidationMessages.MenuNameMaxLength
ValidationMessages.MenuMustHaveSection
ValidationMessages.NewMenuNameRequired

// Section Validation
ValidationMessages.SectionNameRequired
ValidationMessages.SectionNameMaxLength
ValidationMessages.SectionDisplayOrderMinimum
ValidationMessages.SectionMustHaveItem
ValidationMessages.SectionMaxItems
ValidationMessages.SectionAvailableFromLessThanTo

// Menu Item Validation
ValidationMessages.ItemNameRequired
ValidationMessages.ItemNameMaxLength
ValidationMessages.ItemDescriptionMaxLength
ValidationMessages.ItemPriceGreaterThanZero
ValidationMessages.ItemMustHaveCategory
ValidationMessages.ItemMaxCategories

// Ingredient Validation
ValidationMessages.IngredientNameRequired
ValidationMessages.IngredientNameMaxLength
```

## Validation Flow

### Example: UpdateMenu Endpoint

1. **Endpoint** receives request → `/api/cafes/{cafeId}/menus/{menuId}`

2. **ValidationFilter** runs FluentValidation:
   ```csharp
   RuleFor(x => x.CafeId).NotEmpty();
   RuleFor(x => x.MenuId).NotEmpty();
   RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
   ```
   - ❌ Fails → Returns `400 Bad Request` with validation errors
   - ✅ Passes → Continue to handler

3. **Handler** checks existence and business rules:
   ```csharp
   var menu = await menuRepository.GetByIdAsync(request.MenuId, ct);
   
   // Check existence + ownership
   if (menu == null || menu.CafeId != request.CafeId)
       return Result.Failure(Error.NotFound(..., ErrorCodes.MenuNotFound));
   
   // Check business rules (e.g., categories exist)
   if (missingCategories.Any())
       return Result.Failure(Error.Validation(..., ErrorCodes.CategoriesNotFound));
   ```
   - ❌ Fails → Returns `404 Not Found` or `409 Conflict`
   - ✅ Passes → Execute business logic

4. **Endpoint** maps Result to HTTP response:
   ```csharp
   return result.ToApiResult(); // or ToNoContentResult()
   ```

## Result Pattern Mapping

### HTTP Status Code Mapping

```csharp
ErrorType.Validation → 400 Bad Request (ProblemDetails)
ErrorType.NotFound   → 404 Not Found (ProblemDetails)
ErrorType.Conflict   → 409 Conflict (ProblemDetails)
```

### Extension Methods

- `result.ToApiResult()` → `Ok(200)` or error status
- `result.ToCreatedResult(locationFactory)` → `Created(201, location)` or error status
- `result.ToNoContentResult()` → `NoContent(204)` or error status

## Validator Examples

### Simple Format Validation (Standard Pattern)

```csharp
public class DeleteMenuCommandValidator : AbstractValidator<DeleteMenuCommand>
{
    public DeleteMenuCommandValidator()
    {
        RuleFor(x => x.CafeId)
            .NotEmpty().WithMessage(ValidationMessages.CafeIdRequired);

        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage(ValidationMessages.MenuIdRequired);
    }
}
```

### Complex Structure Validation

```csharp
public class CreateMenuRequestValidator : AbstractValidator<CreateMenuRequest>
{
    public CreateMenuRequestValidator()
    {
        RuleFor(x => x.CafeId)
            .NotEmpty().WithMessage(ValidationMessages.CafeIdRequired);
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.MenuNameRequired)
            .MaximumLength(200).WithMessage(ValidationMessages.MenuNameMaxLength);
        
        RuleFor(x => x.Sections)
            .NotEmpty().WithMessage(ValidationMessages.MenuMustHaveSection);
        
        RuleForEach(x => x.Sections)
            .SetValidator(new SectionDtoValidator());
    }
}
```

## Handler Examples

### Existence Check Pattern

```csharp
public async Task<Result<GetMenuResponse>> HandleAsync(
    GetMenuQuery request,
    CancellationToken cancellationToken)
{
    var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

    // Single query: check existence AND ownership
    if (menu == null || menu.CafeId != request.CafeId)
    {
        return Result<GetMenuResponse>.Failure(Error.NotFound(
            $"Menu with ID {request.MenuId} not found",
            ErrorCodes.MenuNotFound));
    }

    // Map and return
    return Result<GetMenuResponse>.Success(...);
}
```

### Business Rule Validation

```csharp
public async Task<Result<ActivateMenuResponse>> HandleAsync(
    ActivateMenuCommand request,
    CancellationToken cancellationToken)
{
    var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

    // Check existence
    if (menu == null || menu.CafeId != request.CafeId)
        return Result<ActivateMenuResponse>.Failure(
            Error.NotFound(..., ErrorCodes.MenuNotFound));

    // Business rule: must be published first
    if (!menu.IsPublished)
        return Result<ActivateMenuResponse>.Failure(
            Error.Conflict(..., ErrorCodes.MenuNotPublished));

    // Business rule: avoid duplicate activation
    if (menu.IsActive)
        return Result<ActivateMenuResponse>.Failure(
            Error.Conflict(..., ErrorCodes.MenuAlreadyActive));

    // Execute business logic
    menu.IsActive = true;
    // ... save and return
}
```

## Repositories

### ICafeRepository
- `Task<bool> ExistsAsync(Guid cafeId, CancellationToken ct)`
  - Used by: `CreateMenuHandler`

### IMenuRepository
- `Task<Menu?> GetByIdAsync(Guid menuId, CancellationToken ct)`
  - Returns full menu entity
  - Handlers check `menu.CafeId` for ownership validation

## Why This Architecture?

### ❌ Previous Approach (Rejected)
- Async database validation in FluentValidation validators
- **Problem**: Validators always return `400 Bad Request`
- **Issue**: Missing resources returned `400` instead of `404`

### ✅ Current Approach (Final)
1. **Validators**: Simple, synchronous, format-only → `400`
2. **Handlers**: Async, database checks, business rules → `404`/`409`
3. **Error Codes**: Centralized constants in Domain layer
4. **Result Pattern**: No exceptions, explicit error handling

### Benefits
- ✅ Correct HTTP status codes (RESTful compliance)
- ✅ Clear separation of concerns
- ✅ Simple, maintainable validators (no base classes needed)
- ✅ Efficient single-query validation (get menu + check ownership)
- ✅ Consistent error codes across all handlers
- ✅ No exceptions for business errors (Result pattern)

## ProblemDetails Response Format

All endpoints produce ProblemDetails for errors:

```csharp
.WithName("CreateMenu")
.WithOpenApi()
.Produces<CreateMenuResponse>(StatusCodes.Status201Created)
.ProducesValidationProblem()           // 400 validation errors
.ProducesProblem(StatusCodes.Status404NotFound)  // 404 not found
```

Example response:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "errors": {
    "code": "MENU_NOT_FOUND",
    "message": "Menu with ID 123 not found"
  }
}
```

## Testing Strategy

### Unit Tests
- **Validators**: Test format validation rules
- **Handlers**: Mock repositories, test business logic

### Integration Tests
- Test full request/response cycle with real database
- Verify correct HTTP status codes
- Validate ProblemDetails structure

## Migration Notes

When implementing new features:

1. **Create Validator** (format only):
   - Inherit from `AbstractValidator<T>`
   - Add format rules using `ValidationMessages` constants
   - Example: `.NotEmpty().WithMessage(ValidationMessages.CafeIdRequired)`
   - NO database dependencies

2. **Create Handler**:
   - Inject repositories via constructor
   - Check resource existence first
   - Return `Error.NotFound` with `ErrorCodes` constants
   - Check business rules
   - Return `Error.Conflict` or `Error.Validation` as appropriate

3. **Create Endpoint**:
   - Use Result extension methods (`ToApiResult`, `ToCreatedResult`)
   - Add `.ProducesValidationProblem()` and `.ProducesProblem(statusCode)`
   - Add `.WithOpenApi()` for documentation

4. **Add Error Codes & Messages**:
   - Add error code constants to `Domain/ErrorCodes.cs`
   - Add validation messages to `Application/Common/Validators/ValidationMessages.cs`
   - Group by error type (404, 400, 409)
   - Use consistent naming (`{Resource}{Condition}`)
