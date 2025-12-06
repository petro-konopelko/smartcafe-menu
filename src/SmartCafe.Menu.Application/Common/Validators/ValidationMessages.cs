namespace SmartCafe.Menu.Application.Common.Validators;

/// <summary>
/// Centralized validation error messages for FluentValidation.
/// Used for consistent validation messaging across all validators.
/// </summary>
public static class ValidationMessages
{
    // Common ID Validation
    public const string CafeIdRequired = "Cafe ID is required";
    public const string MenuIdRequired = "Menu ID is required";
    public const string SourceMenuIdRequired = "Source menu ID is required";

    // Menu Validation
    public const string MenuNameRequired = "Menu name is required";
    public const string MenuNameMaxLength = "Menu name must not exceed 200 characters";
    public const string MenuMustHaveSection = "Menu must have at least one section";
    public const string NewMenuNameRequired = "New menu name is required";

    // Section Validation
    public const string SectionNameRequired = "Section name is required";
    public const string SectionNameMaxLength = "Section name must not exceed 100 characters";
    public const string SectionDisplayOrderMinimum = "Display order must be greater than or equal to 0";
    public const string SectionMustHaveItem = "Section must have at least one item";
    public const string SectionMaxItems = "Section cannot have more than 100 items";
    public const string SectionAvailableFromLessThanTo = "AvailableFrom must be less than AvailableTo";

    // Menu Item Validation
    public const string ItemNameRequired = "Item name is required";
    public const string ItemNameMaxLength = "Item name must not exceed 200 characters";
    public const string ItemDescriptionMaxLength = "Description must not exceed 500 characters";
    public const string ItemPriceGreaterThanZero = "Price must be greater than 0";
    public const string ItemMustHaveCategory = "Item must have at least one category";
    public const string ItemMaxCategories = "Item cannot have more than 10 categories";

    // Price Validation
    public const string PriceRequired = "Price is required";
    public const string PriceUnitInvalid = "Price unit must be a valid value";
    public const string DiscountInvalid = "Discount must be between 0 and 1";

    // Ingredient Validation
    public const string IngredientNameRequired = "Ingredient name is required";
    public const string IngredientNameMaxLength = "Ingredient name must not exceed 100 characters";
}
