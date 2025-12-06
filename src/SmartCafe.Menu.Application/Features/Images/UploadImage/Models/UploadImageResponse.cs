namespace SmartCafe.Menu.Application.Features.Images.UploadImage;

public record UploadImageResponse(
    string OriginalImageUrl,
    string ThumbnailImageUrl
);
