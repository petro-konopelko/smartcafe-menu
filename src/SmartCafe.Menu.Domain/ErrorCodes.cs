namespace SmartCafe.Menu.Domain;

/// <summary>
/// Centralized error codes for the Menu Service domain.
/// Used for consistent error reporting across handlers and APIs.
/// </summary>
public static class ErrorCodes
{
    // Resource Not Found Errors (404)
    public const string CafeNotFound = "CAFE_NOT_FOUND";
    public const string MenuNotFound = "MENU_NOT_FOUND";
    public const string CategoryNotFound = "CATEGORY_NOT_FOUND";

    // Validation Errors (400)
    public const string CategoriesNotFound = "CATEGORIES_NOT_FOUND";

    // Conflict Errors (409)
    public const string MenuAlreadyActive = "MENU_ALREADY_ACTIVE";
    public const string MenuAlreadyPublished = "MENU_ALREADY_PUBLISHED";
    public const string MenuNotPublished = "MENU_NOT_PUBLISHED";
    public const string CannotDeletePublishedMenu = "CANNOT_DELETE_PUBLISHED_MENU";
}
