using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Domain.ValueObjects;

/// <summary>
/// Represents relative paths for image storage (original + thumbnail) for a MenuItem.
/// Relative paths allow storage account and container changes without database updates.
/// Example: "cafeId/menuId/itemId/original.jpg"
/// </summary>
public class ImageAsset
{
    public string? OriginalPath { get; }
    public string? ThumbnailPath { get; }

    private ImageAsset(string? originalPath, string? thumbnailPath)
    {
        OriginalPath = originalPath;
        ThumbnailPath = thumbnailPath;
    }

    internal static Result<ImageAsset> Create(string? originalPath, string? thumbnailPath)
    {
        var original = originalPath?.Trim();
        var thumbnail = thumbnailPath?.Trim();

        var bothMissing = string.IsNullOrWhiteSpace(original) && string.IsNullOrWhiteSpace(thumbnail);
        var bothPresent = !string.IsNullOrWhiteSpace(original) && !string.IsNullOrWhiteSpace(thumbnail);

        if (bothMissing)
        {
            return Result<ImageAsset>.Success(new ImageAsset(null, null));
        }

        if (!bothPresent)
        {
            return Result<ImageAsset>.Failure(
                Error.Validation(
                    new ErrorDetail(
                        "Both OriginalPath and ThumbnailPath must be provided together.",
                        ItemErrorCodes.ImageAssetPathsInvalid)));
        }

        return Result<ImageAsset>.Success(new ImageAsset(original, thumbnail));
    }
}
