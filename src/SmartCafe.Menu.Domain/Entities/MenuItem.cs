using SmartCafe.Menu.Domain.Common;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Domain.Models;
using SmartCafe.Menu.Domain.ValueObjects;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Domain.Entities;

public class MenuItem : Entity
{
    public Guid SectionId { get; }
    public Section Section { get; init; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Price Price { get; private set; } = null!;
    public ImageAsset? Image { get; private set; }
    public List<Ingredient> IngredientOptions { get; private set; } = [];
    public int Position { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; private set; }

    // Is required for EF Core
    private MenuItem() { }

    private MenuItem(
        Guid menuItemId,
        Guid sectionId)
    {
        Id = menuItemId;
        SectionId = sectionId;
    }

    internal static MenuItem Create(Guid itemId, Guid sectionId, DateTime createdAt)
    {
        if (itemId.Equals(Guid.Empty))
        {
            throw new ArgumentException("ID cannot be empty", nameof(itemId));
        }

        if (sectionId.Equals(Guid.Empty))
        {
            throw new ArgumentException("Section ID cannot be empty", nameof(sectionId));
        }

        if (createdAt == default)
        {
            throw new ArgumentException("CreatedAt must be a valid date", nameof(createdAt));
        }

        return new MenuItem(itemId, sectionId)
        {
            CreatedAt = createdAt
        };
    }

    internal Result UpdateDetails(
        ItemUpdateInfo itemInfo,
        int position,
        DateTime updatedAt)
    {
        var errors = new List<ErrorDetail>();

        if (position < 0)
        {
            throw new ArgumentException("Position cannot be negative", nameof(position));
        }

        if (updatedAt == default)
        {
            throw new ArgumentException("UpdatedAt must be a valid date", nameof(updatedAt));
        }

        var trimmedName = itemInfo.Name?.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            errors.Add(new ErrorDetail("Item name is required", ItemErrorCodes.ItemNameRequired));
        }

        Price? price = null;

        if (itemInfo.Price is null)
        {
            errors.Add(new ErrorDetail("Price is required", ItemErrorCodes.PriceAmountRequired));
        }
        else
        {
            var priceResult = Price.Create(
                itemInfo.Price.Amount,
                itemInfo.Price.Unit,
                itemInfo.Price.Discount);

            if (priceResult.IsFailure)
            {
                errors.AddRange(priceResult.EnsureError().Details);
            }
            else
            {
                price = priceResult.EnsureValue();
            }
        }

        var imageResult = ImageAsset.Create(
            itemInfo.Image?.OriginalPath,
            itemInfo.Image?.ThumbnailPath);

        if (imageResult.IsFailure)
        {
            errors.AddRange(imageResult.EnsureError().Details);
        }

        // Update ingredients
        var ingredientResults = itemInfo.Ingredients
            .Select(ig => Ingredient.Create(ig.Name, ig.IsExcludable))
            .ToArray();

        if (ingredientResults.Any(x => x.IsFailure))
        {
            var ingredientErrorDetails = ingredientResults
                .Where(x => x.IsFailure)
                .SelectMany(x => x.EnsureError().Details);

            errors.AddRange(ingredientErrorDetails);
        }

        if (errors.Count > 0)
        {
            return Result.Failure(Error.Validation(errors));
        }

        Name = trimmedName!;
        Description = itemInfo.Description?.Trim();
        Price = price ?? throw new ArgumentNullException(nameof(price), "Price must be set");
        Image = imageResult.EnsureValue();
        Position = position;
        UpdatedAt = updatedAt;
        IngredientOptions = [.. ingredientResults.Select(x => x.EnsureValue())];

        return Result.Success();
    }
}
