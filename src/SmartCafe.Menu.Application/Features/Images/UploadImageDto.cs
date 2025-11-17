using Microsoft.AspNetCore.Http;

namespace SmartCafe.Menu.Application.Features.Images;

public record UploadImageRequest(
    Guid CafeId,
    Guid MenuId,
    Guid ItemId,
    IFormFile Image
);

public record UploadImageResponse(
    string FullImageUrl,
    string CroppedImageUrl
);
