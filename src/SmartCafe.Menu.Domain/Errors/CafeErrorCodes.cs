namespace SmartCafe.Menu.Domain.Errors;

public static class CafeErrorCodes
{
    // Not Found Errors (404)
    public const string CafeNotFound = "CAFE_NOT_FOUND";

    // Validation Errors (400)
    public const string CafeNameRequired = "CAFE_NAME_REQUIRED";
    public const string CafeNameTooLong = "CAFE_NAME_TOO_LONG";
    public const string CafeContactInfoTooLong = "CAFE_CONTACT_INFO_TOO_LONG";

    // Conflict Errors (409)
    public const string CafeAlreadyDeleted = "CAFE_ALREADY_DELETED";
}
