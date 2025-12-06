using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Images.UploadImage;

public class UploadImageHandler(IImageStorageService imageStorageService) : ICommandHandler<UploadImageCommand, UploadImageResponse>
{
    public async Task<UploadImageResponse> HandleAsync(
        UploadImageCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        // Extract file extension from filename or content type
        var fileExtension = Path.GetExtension(command.FileName)?.TrimStart('.') ?? string.Empty;
        if (string.IsNullOrEmpty(fileExtension))
        {
            // Fallback to content type
            fileExtension = command.ContentType.Split('/').Last();
        }

        var (originalImagePath, thumbnailImagePath) = await imageStorageService.UploadItemImageAsync(
            command.CafeId,
            command.MenuId,
            command.ItemId,
            command.ImageStream,
            fileExtension,
            cancellationToken);

        // Construct absolute URLs from paths
        var originalImageUrl = imageStorageService.GetAbsoluteUrl(originalImagePath);
        var thumbnailImageUrl = imageStorageService.GetAbsoluteUrl(thumbnailImagePath);

        return new UploadImageResponse(OriginalImageUrl: originalImageUrl, ThumbnailImageUrl: thumbnailImageUrl);
    }
}
