using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SmartCafe.Menu.Application.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace SmartCafe.Menu.Infrastructure.BlobStorage;

public class AzureBlobStorageService(BlobServiceClient blobServiceClient, string containerName, IImageProcessor imageProcessor) : IImageStorageService
{
    private readonly BlobContainerClient _containerClient = blobServiceClient.GetBlobContainerClient(containerName);

    public async Task<(string FullImageUrl, string CroppedImageUrl)> UploadItemImageAsync(
        Guid cafeId,
        Guid menuId,
        Guid itemId,
        Stream imageStream,
        string fileExtension,
        CancellationToken cancellationToken = default)
    {
        await _containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        // Normalize extension
        var ext = fileExtension.StartsWith('.') ? fileExtension : $".{fileExtension}";
        
        // Create folder path: cafeId/menuId/itemId/
        var folderPath = $"{cafeId}/{menuId}/{itemId}";
        var fullImageName = $"{folderPath}/full{ext}";
        var croppedImageName = $"{folderPath}/cropped{ext}";

        // Upload full image
        var fullBlobClient = _containerClient.GetBlobClient(fullImageName);
        imageStream.Position = 0;
        await fullBlobClient.UploadAsync(
            imageStream,
            new BlobHttpHeaders { ContentType = GetContentType(ext) },
            cancellationToken: cancellationToken);

        // Generate and upload cropped image
        imageStream.Position = 0;
        using var croppedStream = await imageProcessor.CreateCroppedImageAsync(imageStream, 300, 300, cancellationToken);
        var croppedBlobClient = _containerClient.GetBlobClient(croppedImageName);
        await croppedBlobClient.UploadAsync(
            croppedStream,
            new BlobHttpHeaders { ContentType = GetContentType(ext) },
            cancellationToken: cancellationToken);

        return (fullBlobClient.Uri.ToString(), croppedBlobClient.Uri.ToString());
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
