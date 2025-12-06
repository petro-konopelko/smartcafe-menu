namespace SmartCafe.Menu.Application.Interfaces;

public interface IImageStorageService
{
    /// <summary>
    /// Upload menu item image and generate thumbnail version.
    /// Returns relative paths for both original and thumbnail images.
    /// Path: {cafeId}/{menuId}/{itemId}/original.{ext} and thumbnail.{ext}
    /// </summary>
    Task<(string OriginalImagePath, string ThumbnailImagePath)> UploadItemImageAsync(
        Guid cafeId,
        Guid menuId,
        Guid itemId,
        Stream imageStream,
        string fileExtension,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete all images for a specific menu (entire folder).
    /// Path: {cafeId}/{menuId}/
    /// </summary>
    Task DeleteMenuImagesAsync(Guid cafeId, Guid menuId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete all images for a specific item.
    /// Path: {cafeId}/{menuId}/{itemId}/
    /// </summary>
    Task DeleteItemImagesAsync(Guid cafeId, Guid menuId, Guid itemId, CancellationToken cancellationToken = default);

    Task<string> UploadImageAsync(Stream imageStream, string fileName, CancellationToken cancellationToken = default);
    Task DeleteImageAsync(string blobName, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Construct absolute URL from relative path using current storage configuration.
    /// </summary>
    string GetAbsoluteUrl(string relativePath);
}
