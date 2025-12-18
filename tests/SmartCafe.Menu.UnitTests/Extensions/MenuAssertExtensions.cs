using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.Domain.Models;
using SmartCafe.Menu.Domain.ValueObjects;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.UnitTests.Extensions;

public static class MenuAssertExtensions
{
    public static void VerifyMenu(
        this MenuEntity menu,
        Guid cafeId,
        string menuName,
        DateTime createdAt,
        DateTime updatedAt,
        IReadOnlyCollection<SectionUpdateInfo> expectedSections)
    {
        Assert.NotNull(menu);

        Assert.Multiple(
            () => Assert.Equal(menuName, menu.Name),
            () => Assert.Equal(cafeId, menu.CafeId),
            () => Assert.Equal(MenuState.New, menu.State),
            () => Assert.Equal(createdAt, menu.CreatedAt),
            () => Assert.Equal(updatedAt, menu.UpdatedAt),
            () => Assert.Null(menu.PublishedAt),
            () => Assert.Null(menu.ActivatedAt),
            () => Assert.Equal(expectedSections.Count, menu.Sections.Count));

        Assert.All(expectedSections, (expectedSection, index) =>
        {
            var actualSection = menu.Sections.First(s => s.Position == index + 1);
            Assert.Equal(index + 1, actualSection.Position);

            actualSection.VerifySection(
                menu.Id,
                expectedSection,
                createdAt,
                updatedAt);
        });
    }

    private static void VerifySection(
        this Section section,
        Guid menuId,
        SectionUpdateInfo expectedSection,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Assert.Multiple(
            () => Assert.Equal(expectedSection.Name, section.Name),
            () => Assert.Equal(expectedSection.Items.Count, section.Items.Count),
            () => Assert.Equal(menuId, section.MenuId),
            () => Assert.Equal(expectedSection.Id.HasValue ? createdAt : updatedAt, section.CreatedAt),
            () => Assert.Equal(updatedAt, section.UpdatedAt));

        Assert.All(expectedSection.Items, (expectedItem, index) =>
        {
            var actualItem = section.Items.First(i => i.Position == index + 1);
            Assert.Equal(index + 1, actualItem.Position);

            actualItem.VerifyItem(
                section.Id,
                expectedItem,
                createdAt,
                updatedAt);
        });
    }

    private static void VerifyItem(
        this MenuItem item,
        Guid sectionId,
        ItemUpdateInfo expectedItem,
        DateTime createdAt,
        DateTime updatedAt)
    {
        Assert.Multiple(
            () => Assert.Equal(expectedItem.Name, item.Name),
            () => Assert.Equal(expectedItem.Description, item.Description),
            () => Assert.Equal(sectionId, item.SectionId),
            () => Assert.Equal(expectedItem.Id.HasValue ? createdAt : updatedAt, item.CreatedAt),
            () => Assert.Equal(updatedAt, item.UpdatedAt),
            () => item.Price.VerifyPrice(expectedItem.Price),
            () => item.Image.VerifyImage(expectedItem.Image),
            () => Assert.Equal(expectedItem.Ingredients.Count, item.IngredientOptions.Count));

        Assert.All(item.IngredientOptions, (ingredient, index) =>
        {
            var expectedIngredient = expectedItem.Ingredients.ElementAt(index);
            Assert.Multiple(
                () => Assert.Equal(expectedIngredient.Name, ingredient.Name),
                () => Assert.Equal(expectedIngredient.IsExcludable, ingredient.IsExcludable));
        });
    }

    private static void VerifyPrice(
        this Price price,
        PriceUpdateInfo expectedPrice)
    {
        Assert.Multiple(
            () => Assert.Equal(expectedPrice.Amount, price.Amount),
            () => Assert.Equal(expectedPrice.Unit, price.Unit),
            () => Assert.Equal(expectedPrice.Discount, price.Discount));
    }

    private static void VerifyImage(
        this ImageAsset? image,
        ImageUpdateInfo? expectedImage)
    {
        if (expectedImage is null)
        {
            Assert.Null(image);
        }
        else
        {
            Assert.NotNull(image);
            Assert.Multiple(
                () => Assert.Equal(expectedImage.OriginalPath, image!.OriginalPath),
                () => Assert.Equal(expectedImage.ThumbnailPath, image.ThumbnailPath));
        }
    }
}
