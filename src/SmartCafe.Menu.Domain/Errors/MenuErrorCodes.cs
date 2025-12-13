namespace SmartCafe.Menu.Domain.Errors;

public static class MenuErrorCodes
{
    // Resource Not Found Errors (404)
    public const string MenuNotFound = "MENU_NOT_FOUND";

    // Validation Errors (400)
    public const string MenuHasNoSections = "MENU_HAS_NO_SECTIONS";
    public const string MenuHasNoItems = "MENU_HAS_NO_ITEMS";
    public const string MenuNameRequired = "MENU_NAME_REQUIRED";

    // Conflict Errors (409)
    public const string MenuAlreadyActive = "MENU_ALREADY_ACTIVE";
    public const string MenuAlreadyPublished = "MENU_ALREADY_PUBLISHED";
    public const string MenuNotPublished = "MENU_NOT_PUBLISHED";
    public const string MenuNotActive = "MENU_NOT_ACTIVE";
    public const string CannotDeleteActiveMenu = "CANNOT_DELETE_ACTIVE_MENU";
}
