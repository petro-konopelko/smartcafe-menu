namespace SmartCafe.Menu.Application.Interfaces;

public interface IImageStorageService
{
    /// <summary>
    /// Upload menu item image and generate cropped version.
    /// Returns URLs for both full and cropped images.
    /// Path: {cafeId}/{menuId}/{itemId}/full.{ext} and cropped.{ext}
    /// </summary>
    Task<(string FullImageUrl, string CroppedImageUrl)> UploadItemImageAsync(
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
}
