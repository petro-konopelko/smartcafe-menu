using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;
using MenuItemEntity = SmartCafe.Menu.Domain.Entities.MenuItem;
using SectionEntity = SmartCafe.Menu.Domain.Entities.Section;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Mappers;

public static class MenuMapper
{
    public static MenuDto ToMenuDto(this MenuEntity menu)
    {
        ArgumentNullException.ThrowIfNull(menu);

        return new MenuDto(
            menu.Id,
            menu.Name,
            menu.State,
            menu.Sections.Select(s => s.ToSectionDto()).ToList(),
            menu.CreatedAt,
            menu.UpdatedAt
        );
    }

    public static CreateMenuResponse ToCreateMenuResponse(this MenuEntity menu)
    {
        ArgumentNullException.ThrowIfNull(menu);

        return new CreateMenuResponse(menu.Id, menu.CafeId);
    }

    private static SectionDto ToSectionDto(this SectionEntity section)
    {
        ArgumentNullException.ThrowIfNull(section);

        return new SectionDto(
            section.Id,
            section.Name,
            section.DisplayOrder,
            section.AvailableFrom,
            section.AvailableTo,
            section.Items.Select(i => i.ToMenuItemDto()).ToList()
        );
    }

    private static MenuItemDto ToMenuItemDto(this MenuItemEntity item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return new MenuItemDto(
            item.Id,
            item.Name,
            item.Description,
            item.Price,
            item.Image?.ToImageItemDto(),
            item.IsActive,
            item.MenuItemCategories.Select(c => c.CategoryId).ToList(),
            item.IngredientOptions.Select(ing => ing.ToIngredientDto()).ToList()
        );
    }

    private static ImageItemDto ToImageItemDto(this Domain.ValueObjects.ImageAsset image)
    {
        ArgumentNullException.ThrowIfNull(image);

        return new ImageItemDto(image.BigUrl, image.CroppedUrl);
    }

    private static IngredientDto ToIngredientDto(this Domain.ValueObjects.Ingredient ingredient)
    {
        ArgumentNullException.ThrowIfNull(ingredient);

        return new IngredientDto(
            ingredient.Name,
            ingredient.IsExcludable,
            ingredient.IsIncludable
        );
    }
}
