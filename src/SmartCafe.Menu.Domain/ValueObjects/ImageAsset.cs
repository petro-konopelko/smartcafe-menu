using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.Domain.ValueObjects;

/// <summary>
/// Represents a pair of image blob names (big + cropped) for a MenuItem.
/// Responsible for validating extension and consistent naming convention.
/// </summary>
public sealed record ImageAsset(string BigName, string CroppedName)
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        "jpg", "jpeg", "png", "webp"
    };

    public static Result<ImageAsset> Create(Guid itemId, string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return Result<ImageAsset>.Failure(Error.Validation(new ErrorDetail("Image extension is required", ErrorCodes.ImageInvalidFormat)));

        var ext = extension.Trim().TrimStart('.');
        if (!AllowedExtensions.Contains(ext))
            return Result<ImageAsset>.Failure(Error.Validation(new ErrorDetail($"Unsupported image format '{ext}'", ErrorCodes.ImageInvalidFormat)));

        var big = BuildBigName(itemId, ext);
        var cropped = BuildCroppedName(itemId, ext);
        return Result<ImageAsset>.Success(new ImageAsset(big, cropped));
    }

    public static string BuildBigName(Guid itemId, string extension) => $"{itemId}_big.{extension}";
    public static string BuildCroppedName(Guid itemId, string extension) => $"{itemId}_cropped.{extension}";
}
