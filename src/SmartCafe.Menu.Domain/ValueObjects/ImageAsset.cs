namespace SmartCafe.Menu.Domain.ValueObjects;

/// <summary>
/// Represents a pair of image URLs (big + cropped) for a MenuItem.
/// </summary>
public sealed record ImageAsset(string BigUrl, string CroppedUrl)
{
    public static ImageAsset Create(string bigUrl, string croppedUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bigUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(croppedUrl);

        return new ImageAsset(bigUrl, croppedUrl);
    }
}
