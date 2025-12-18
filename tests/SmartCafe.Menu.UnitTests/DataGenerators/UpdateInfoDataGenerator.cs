using Bogus;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.Domain.Models;

namespace SmartCafe.Menu.UnitTests.DataGenerators;

public class UpdateInfoDataGenerator
{
    private static readonly Faker<IngredientItemUpdate> ingredientFaker = new Faker<IngredientItemUpdate>()
        .CustomInstantiator(f => new IngredientItemUpdate(
            f.Commerce.ProductName(),
            f.Random.Bool()));

    private static readonly Faker<ImageUpdateInfo> imageFaker = new Faker<ImageUpdateInfo>()
        .CustomInstantiator(f =>
        {
            var originalImageUrl = f.Image.PlaceImgUrl().OrDefault(f);
            var thumbnailImageUrl = string.IsNullOrEmpty(originalImageUrl)
                ? null
                : originalImageUrl + "?thumbnail=true";

            return new ImageUpdateInfo(originalImageUrl, thumbnailImageUrl);
        });

    private static readonly Func<Guid?, Faker<ItemUpdateInfo>> itemFaker = itemId => new Faker<ItemUpdateInfo>()
        .CustomInstantiator(f =>
        {
            return new ItemUpdateInfo(
                itemId,
                f.Commerce.ProductName(),
                f.Commerce.ProductDescription(),
                new PriceUpdateInfo(
                    f.Random.Decimal(1, 100),
                    f.PickRandom<PriceUnit>(),
                    f.Random.Decimal(0, 0.99m)
                ),
                imageFaker.Generate(),
                ingredientFaker.Generate(f.Random.Int(1, 5)));
        });

    private static readonly Func<Guid?, int, Guid?[], Faker<SectionUpdateInfo>> sectionFaker = (sectionId, itemCount, itemIds) => new Faker<SectionUpdateInfo>()
        .CustomInstantiator(f =>
        {
            var availableTo = f.Date.Timespan();
            var availableFrom = f.Date.Timespan(availableTo);
            var items = Enumerable.Range(0, itemCount)
                .Select(index => itemFaker(
                    itemIds is not null && itemIds.Length > index ? itemIds[index] : null).Generate())
                .ToArray();

            return new SectionUpdateInfo(
                sectionId,
                f.Commerce.Department(),
                availableFrom,
                availableTo,
                items);
        });

    public static SectionUpdateInfo GenerateUpdateSectionInfo(Guid? sectionId = null, int itemCount = 1, params Guid?[] itemIds)
    {
        var section = sectionFaker(sectionId, itemCount, itemIds).Generate();
        return section;
    }
}
