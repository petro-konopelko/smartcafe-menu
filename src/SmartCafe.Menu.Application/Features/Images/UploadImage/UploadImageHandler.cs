using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Images.UploadImage;

public class UploadImageHandler(IImageStorageService imageStorageService) : ICommandHandler<UploadImageCommand, UploadImageResponse>
{
    public async Task<UploadImageResponse> HandleAsync(
        UploadImageCommand command,
        CancellationToken cancellationToken = default)
    {
        // Extract file extension from filename or content type
        var fileExtension = Path.GetExtension(command.FileName).TrimStart('.');
        if (string.IsNullOrEmpty(fileExtension))
        {
            // Fallback to content type
            fileExtension = command.ContentType.Split('/').Last();
        }

        var (fullImageUrl, croppedImageUrl) = await imageStorageService.UploadItemImageAsync(
            command.CafeId,
            command.MenuId,
            command.ItemId,
            command.ImageStream,
            fileExtension,
            cancellationToken);

        return new UploadImageResponse(fullImageUrl, croppedImageUrl);
    }
}
