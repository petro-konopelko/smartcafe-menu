namespace SmartCafe.Menu.Application.Interfaces;

public interface IImageProcessor
{
    Task<Stream> CreateCroppedImageAsync(Stream originalImage, int width, int height, CancellationToken cancellationToken = default);
    bool IsValidImageFormat(string fileName);
    bool IsValidImageSize(long fileSize);
}
