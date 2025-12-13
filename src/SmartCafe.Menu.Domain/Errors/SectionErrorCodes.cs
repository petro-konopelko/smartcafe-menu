namespace SmartCafe.Menu.Domain.Errors;

public static class SectionErrorCodes
{
    public const string SectionNotFound = "SECTION_NOT_FOUND";
    public const string SectionNameRequired = "SECTION_NAME_REQUIRED";
    public const string SectionNameNotUnique = "SECTION_NAME_NOT_UNIQUE";
    public const string TooManyItems = "SECTION_TOO_MANY_ITEMS";
    public const string InvalidAvailabilityWindow = "INVALID_AVAILABILITY_WINDOW";
    public const string DuplicateSectionId = "DUPLICATE_SECTION_ID";
}
