namespace SmartCafe.Menu.Domain.Models;

/// <summary>
/// Update info for an image with individual paths for validation in domain.
/// Both paths are provided or none (null).
/// </summary>
public record ImageUpdateInfo(
    string? OriginalPath,
    string? ThumbnailPath
);
