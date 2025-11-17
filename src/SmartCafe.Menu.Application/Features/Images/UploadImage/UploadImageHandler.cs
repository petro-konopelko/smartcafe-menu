using SmartCafe.Menu.Application.Interfaces;

namespace SmartCafe.Menu.Application.Features.Images.UploadImage;

public class UploadImageHandler(IImageStorageService imageStorageService)
{
    public async Task<UploadImageResponse> HandleAsync(
        Guid cafeId,
        Guid menuId,
        Guid itemId,
        Stream imageStream,
        string fileExtension,
        CancellationToken cancellationToken = default)
    {
        var (fullImageUrl, croppedImageUrl) = await imageStorageService.UploadItemImageAsync(
            cafeId,
            menuId,
            itemId,
            imageStream,
            fileExtension,
            cancellationToken);

        return new UploadImageResponse(fullImageUrl, croppedImageUrl);
    }
}
