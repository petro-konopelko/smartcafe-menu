namespace SmartCafe.Menu.Domain.ValueObjects;

/// <summary>
/// Represents relative paths for image storage (original + thumbnail) for a MenuItem.
/// Relative paths allow storage account and container changes without database updates.
/// Example: "cafeId/menuId/itemId/original.jpg"
/// </summary>
public sealed record ImageAsset(string OriginalPath, string ThumbnailPath)
{
    public static ImageAsset Create(string originalPath, string thumbnailPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(originalPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(thumbnailPath);

        return new ImageAsset(originalPath, thumbnailPath);
    }
}
