using SmartCafe.Menu.Application.Interfaces;

namespace SmartCafe.Menu.IntegrationTests.Mocks;

public sealed class NoOpImageStorageService : IImageStorageService
{
    public Task<(string OriginalImagePath, string ThumbnailImagePath)> UploadItemImageAsync(
        Guid cafeId,
        Guid menuId,
        Guid itemId,
        Stream imageStream,
        string fileExtension,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(($"{cafeId}/{menuId}/{itemId}/original.{fileExtension}", $"{cafeId}/{menuId}/{itemId}/thumbnail.{fileExtension}"));
    }

    public Task DeleteMenuImagesAsync(Guid cafeId, Guid menuId, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task DeleteItemImagesAsync(Guid cafeId, Guid menuId, Guid itemId, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<string> UploadImageAsync(Stream imageStream, string fileName, CancellationToken cancellationToken = default)
        => Task.FromResult(fileName);

    public Task DeleteImageAsync(string blobName, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task<bool> ExistsAsync(string blobName, CancellationToken cancellationToken = default) => Task.FromResult(false);

    public string GetAbsoluteUrl(string relativePath) => relativePath;
}
