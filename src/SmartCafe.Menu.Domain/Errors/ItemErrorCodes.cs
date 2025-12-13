namespace SmartCafe.Menu.Domain.Errors;

public static class ItemErrorCodes
{
    public const string ItemNotFound = "ITEM_NOT_FOUND";
    public const string ItemNameRequired = "ITEM_NAME_REQUIRED";
    public const string PriceAmountRequired = "PRICE_AMOUNT_REQUIRED";
    public const string PriceInvalid = "PRICE_INVALID";
    public const string PriceDiscountInvalid = "PRICE_DISCOUNT_INVALID";
    public const string ImageAssetPathsInvalid = "IMAGE_ASSET_PATHS_INVALID";
    public const string IngredientNameRequired = "INGREDIENT_NAME_REQUIRED";
    public const string ItemPositionInvalid = "ITEM_POSITION_INVALID";
    public const string UpdatedAtInvalid = "UPDATED_AT_INVALID";
    public const string ItemNameNotUnique = "ITEM_NAME_NOT_UNIQUE";
    public const string DuplicateItemId = "DUPLICATE_ITEM_ID";
}
