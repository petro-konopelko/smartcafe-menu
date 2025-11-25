using SmartCafe.Menu.Application.Features.Images.UploadImage;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.API.Endpoints.Images;

public static class UploadImageEndpoint
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public static RouteGroupBuilder MapUploadImage(this RouteGroupBuilder group)
    {
        group.MapPost("/upload", async (
            HttpRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            // Read form data
            var form = await request.ReadFormAsync(ct);

            if (!form.Files.Any())
            {
                return Results.BadRequest(new { Message = "No image file provided" });
            }

            var imageFile = form.Files[0];
            var cafeIdStr = form["cafeId"].ToString();
            var menuIdStr = form["menuId"].ToString();
            var itemIdStr = form["itemId"].ToString();

            // Validate IDs
            if (!Guid.TryParse(cafeIdStr, out var cafeId) ||
                !Guid.TryParse(menuIdStr, out var menuId) ||
                !Guid.TryParse(itemIdStr, out var itemId))
            {
                return Results.BadRequest(new { Message = "Invalid cafeId, menuId, or itemId" });
            }

            // Validate file
            if (imageFile.Length == 0)
            {
                return Results.BadRequest(new { Message = "Empty file" });
            }

            if (imageFile.Length > MaxFileSize)
            {
                return Results.BadRequest(new { Message = $"File size exceeds {MaxFileSize / 1024 / 1024}MB limit" });
            }

            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return Results.BadRequest(new { Message = $"Invalid file type. Allowed: {string.Join(", ", AllowedExtensions)}" });
            }

            // Upload image
            using var stream = imageFile.OpenReadStream();
            var command = new UploadImageCommand(
                cafeId,
                menuId,
                itemId,
                stream,
                imageFile.FileName,
                imageFile.ContentType
            );
            var result = await mediator.Send<UploadImageCommand, UploadImageResponse>(command, ct);
            return Results.Ok(result);
        })
        .WithName("UploadImage")
        .WithSummary("Upload menu item image (generates full and cropped versions)")
        .DisableAntiforgery() // Required for file uploads
        .Produces<UploadImageResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        return group;
    }
}
