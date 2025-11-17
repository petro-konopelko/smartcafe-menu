namespace SmartCafe.Menu.Application.Features.Images.UploadImage;

public record UploadImageResponse(
    string FullImageUrl,
    string CroppedImageUrl
);
