using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SmartCafe.Menu.Application.Interfaces;

namespace SmartCafe.Menu.Infrastructure.BlobStorage;

public class AzureBlobStorageService(BlobServiceClient blobServiceClient, string containerName, IImageProcessor imageProcessor) : IImageStorageService
{
    private readonly BlobContainerClient _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
    private readonly string _accountName = blobServiceClient.AccountName;
    private readonly string _containerName = containerName;

    public async Task<(string OriginalImagePath, string ThumbnailImagePath)> UploadItemImageAsync(
        Guid cafeId,
        Guid menuId,
        Guid itemId,
        Stream imageStream,
        string fileExtension,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(imageStream);
        ArgumentNullException.ThrowIfNull(fileExtension);

        await _containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        // Normalize extension
        var ext = fileExtension.StartsWith('.') ? fileExtension : $".{fileExtension}";

        // Create folder path: cafeId/menuId/itemId/
        var folderPath = $"{cafeId}/{menuId}/{itemId}";
        var originalImageName = $"{folderPath}/original{ext}";
        var thumbnailImageName = $"{folderPath}/thumbnail{ext}";

        // Upload original image
        var originalBlobClient = _containerClient.GetBlobClient(originalImageName);
        imageStream.Position = 0;
        await originalBlobClient.UploadAsync(
            imageStream,
            new BlobHttpHeaders { ContentType = GetContentType(ext) },
            cancellationToken: cancellationToken);

        // Generate and upload thumbnail image
        imageStream.Position = 0;
        using var thumbnailStream = await imageProcessor.CreateCroppedImageAsync(imageStream, 300, 300, cancellationToken);
        var thumbnailBlobClient = _containerClient.GetBlobClient(thumbnailImageName);
        await thumbnailBlobClient.UploadAsync(
            thumbnailStream,
            new BlobHttpHeaders { ContentType = GetContentType(ext) },
            cancellationToken: cancellationToken);

        return (originalImageName, thumbnailImageName);
    }

    public async Task DeleteMenuImagesAsync(Guid cafeId, Guid menuId, CancellationToken cancellationToken = default)
    {
        var prefix = $"{cafeId}/{menuId}/";
        await foreach (var blobItem in _containerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
        {
            await _containerClient.DeleteBlobIfExistsAsync(blobItem.Name, cancellationToken: cancellationToken);
        }
    }

    public async Task DeleteItemImagesAsync(Guid cafeId, Guid menuId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var prefix = $"{cafeId}/{menuId}/{itemId}/";
        await foreach (var blobItem in _containerClient.GetBlobsAsync(prefix: prefix, cancellationToken: cancellationToken))
        {
            await _containerClient.DeleteBlobIfExistsAsync(blobItem.Name, cancellationToken: cancellationToken);
        }
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, CancellationToken cancellationToken = default)
    {
        await _containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        var blobClient = _containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(
            imageStream,
            new BlobHttpHeaders { ContentType = GetContentType(fileName) },
            cancellationToken: cancellationToken);

        return blobClient.Uri.ToString();
    }

    public async Task DeleteImageAsync(string blobName, CancellationToken cancellationToken = default)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsAsync(string blobName, CancellationToken cancellationToken = default)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        return await blobClient.ExistsAsync(cancellationToken);
    }

    public string GetAbsoluteUrl(string relativePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        return $"https://{_accountName}.blob.core.windows.net/{_containerName}/{relativePath}";
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}
