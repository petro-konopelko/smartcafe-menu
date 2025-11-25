using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Images.UploadImage;

public record UploadImageCommand(
    Guid CafeId,
    Guid MenuId,
    Guid ItemId,
    Stream ImageStream,
    string FileName,
    string ContentType
) : ICommand<UploadImageResponse>;
