using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.ValueObjects;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;
using MenuItemEntity = SmartCafe.Menu.Domain.Entities.MenuItem;
using SectionEntity = SmartCafe.Menu.Domain.Entities.Section;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Mappers;

public static class MenuMapper
{
    public static MenuDto ToMenuDto(this MenuEntity menu, IImageStorageService imageStorageService)
    {
        ArgumentNullException.ThrowIfNull(menu);

        return new MenuDto(
            menu.Id,
            menu.Name,
            menu.State,
            menu.Sections.Select(s => s.ToSectionDto(imageStorageService)).ToList(),
            menu.CreatedAt,
            menu.UpdatedAt
        );
    }

    public static CreateMenuResponse ToCreateMenuResponse(this MenuEntity menu)
    {
        ArgumentNullException.ThrowIfNull(menu);

        return new CreateMenuResponse(menu.Id, menu.CafeId);
    }

    private static SectionDto ToSectionDto(this SectionEntity section, IImageStorageService imageStorageService)
    {
        ArgumentNullException.ThrowIfNull(section);

        return new SectionDto(
            section.Id,
            section.Name,
            section.DisplayOrder,
            section.AvailableFrom,
            section.AvailableTo,
            section.Items.Select(i => i.ToMenuItemDto(imageStorageService)).ToList()
        );
    }

    private static MenuItemDto ToMenuItemDto(this MenuItemEntity item, IImageStorageService imageStorageService)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new MenuItemDto(
            item.Id,
            item.Name,
            item.Description,
            item.Price.ToPriceDto(),
            item.Image?.ToImageItemDto(imageStorageService),
            item.IsActive,
            item.MenuItemCategories.Select(c => c.CategoryId).ToList(),
            item.IngredientOptions.Select(ing => ing.ToIngredientDto()).ToList()
        );
    }

    private static PriceDto ToPriceDto(this Price price)
    {
        ArgumentNullException.ThrowIfNull(price);

        return new PriceDto(price.Amount, price.Unit, price.Discount);
    }

    private static MenuItemImageDto? ToImageItemDto(this ImageAsset? image, IImageStorageService imageStorageService)
    {
        if (image == null)
            return null;

        ArgumentException.ThrowIfNullOrWhiteSpace(image.OriginalPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(image.ThumbnailPath);

        var originalUrl = imageStorageService.GetAbsoluteUrl(image.OriginalPath);
        var thumbnailUrl = imageStorageService.GetAbsoluteUrl(image.ThumbnailPath);

        return new MenuItemImageDto(originalUrl, thumbnailUrl);
    }

    private static IngredientDto ToIngredientDto(this Ingredient ingredient)
    {
        ArgumentNullException.ThrowIfNull(ingredient);

        return new IngredientDto(
            ingredient.Name,
            ingredient.IsExcludable,
            ingredient.IsIncludable
        );
    }
}
