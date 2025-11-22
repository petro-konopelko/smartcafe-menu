using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SmartCafe.Menu.Application.Interfaces;

namespace SmartCafe.Menu.Infrastructure.Services;

public class ImageProcessingService : IImageProcessor
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public async Task<Stream> CreateCroppedImageAsync(Stream originalImage, int width, int height, CancellationToken cancellationToken = default)
    {
        using var image = await Image.LoadAsync(originalImage, cancellationToken);

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = ResizeMode.Crop
        }));

        var outputStream = new MemoryStream();
        await image.SaveAsync(outputStream, new JpegEncoder(), cancellationToken);
        outputStream.Position = 0;

        return outputStream;
    }

    public bool IsValidImageFormat(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }

    public bool IsValidImageSize(long fileSize) => fileSize <= MaxFileSize;
}
