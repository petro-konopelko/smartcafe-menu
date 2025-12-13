using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Domain.Models;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Mappers;

/// <summary>
/// Mapper to convert DTOs to domain update models.
/// </summary>
public static class SectionUpdateMapper
{
    /// <summary>
    /// Maps a list of SectionDto to a list of SectionUpdateInfo.
    /// </summary>
    public static IReadOnlyCollection<SectionUpdateInfo> ToSectionUpdateInfoCollection(this IEnumerable<SectionDto> dtos)
    {
        ArgumentNullException.ThrowIfNull(dtos);

        return [.. dtos.Select(dto => dto.ToSectionUpdateInfo())];
    }

    /// <summary>
    /// Maps a SectionDto to a SectionUpdateInfo domain model.
    /// </summary>
    public static SectionUpdateInfo ToSectionUpdateInfo(this SectionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new SectionUpdateInfo(
            dto.Id,
            dto.Name,
            dto.AvailableFrom,
            dto.AvailableTo,
            dto.Items.ToItemUpdateInfoList()
        );
    }

    /// <summary>
    /// Maps a MenuItemDto to an ItemUpdateInfo domain model.
    /// </summary>
    private static ItemUpdateInfo ToItemUpdateInfo(this MenuItemDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var price = new PriceUpdateInfo(dto.Price.Amount, dto.Price.Unit, dto.Price.Discount);

        var image = dto.Image is null
            ? null
            : new ImageUpdateInfo(dto.Image.OriginalImageUrl, dto.Image.ThumbnailImageUrl);

        return new ItemUpdateInfo(
            dto.Id,
            dto.Name,
            dto.Description,
            price,
            image,
            [.. dto.Ingredients.Select(i => i.ToIngredientItemUpdate())]
        );
    }

    /// <summary>
    /// Maps a list of MenuItemDto to a list of ItemUpdateInfo.
    /// </summary>
    private static IReadOnlyCollection<ItemUpdateInfo> ToItemUpdateInfoList(this IEnumerable<MenuItemDto> dtos)
    {
        ArgumentNullException.ThrowIfNull(dtos);

        return [.. dtos.Select(dto => dto.ToItemUpdateInfo())];
    }

    /// <summary>
    /// Maps an IngredientDto to an IngredientItemUpdate model.
    /// </summary>
    private static IngredientItemUpdate ToIngredientItemUpdate(this IngredientDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new IngredientItemUpdate(dto.Name, dto.IsExcludable);
    }
}
