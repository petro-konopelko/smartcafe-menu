using SmartCafe.Menu.Application.Features.Menus.CloneMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Mappers;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Domain.Models;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Shared.Providers.Abstractions;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.Application.Features.Menus.CloneMenu;

public class CloneMenuHandler(
    IMenuRepository menuRepository,
    ICafeRepository cafeRepository,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher,
    IDateTimeProvider dateTimeProvider,
    IGuidIdProvider guidIdProvider) : ICommandHandler<CloneMenuCommand, Result<CreateMenuResponse>>
{
    public async Task<Result<CreateMenuResponse>> HandleAsync(
        CloneMenuCommand request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Check cafe exists (not deleted)
        var cafeExists = await cafeRepository.ExistsActiveAsync(request.CafeId, cancellationToken);
        if (!cafeExists)
        {
            return Result<CreateMenuResponse>.Failure(Error.NotFound(
                $"Cafe with ID {request.CafeId} not found",
                CafeErrorCodes.CafeNotFound));
        }

        // Load source menu
        var sourceMenu = await menuRepository.GetByIdAsync(request.SourceMenuId, cancellationToken);
        if (sourceMenu is null || sourceMenu.CafeId != request.CafeId)
        {
            return Result<CreateMenuResponse>.Failure(Error.NotFound(
                $"Menu with ID {request.SourceMenuId} not found",
                MenuErrorCodes.MenuNotFound));
        }

        // Create cloned menu via domain factory
        var clonedMenuResult = MenuEntity.Create(
            request.CafeId,
            request.NewName,
            guidIdProvider,
            dateTimeProvider,
            [.. sourceMenu.Sections.OrderBy(s => s.Position).Select(ToUpdateSectionInfo)]);

        if (clonedMenuResult.IsFailure)
        {
            return Result<CreateMenuResponse>.Failure(clonedMenuResult.EnsureError());
        }

        var clonedMenu = clonedMenuResult.EnsureValue();

        // Save cloned menu
        await menuRepository.CreateAsync(clonedMenu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Dispatch domain events directly from cloned menu
        var events = clonedMenu.DomainEvents.ToList();
        clonedMenu.ClearDomainEvents();
        await eventDispatcher.DispatchAsync(events, cancellationToken);

        return Result<CreateMenuResponse>.Success(clonedMenu.ToCreateMenuResponse());
    }

    private SectionUpdateInfo ToUpdateSectionInfo(Section section)
    {
        ArgumentNullException.ThrowIfNull(section);

        // Order items to preserve position in SyncCollection
        var itemUpdateInfos = section.Items
            .OrderBy(i => i.Position)
            .Select(ToUpdateItemInfo)
            .ToArray();

        return new SectionUpdateInfo(
            Id: null, // exclude existing ID for cloning
            Name: section.Name,
            AvailableFrom: section.AvailableFrom,
            AvailableTo: section.AvailableTo,
            Items: itemUpdateInfos);
    }

    private ItemUpdateInfo ToUpdateItemInfo(MenuItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var price = new PriceUpdateInfo(
            item.Price.Amount,
            item.Price.Unit,
            item.Price.Discount);

        var image = item.Image is null
            ? null
            : new ImageUpdateInfo(item.Image.OriginalPath, item.Image.ThumbnailPath);

        var ingredients = item.IngredientOptions
            .Select(ing => new IngredientItemUpdate(ing.Name, ing.IsExcludable))
            .ToArray();

        return new ItemUpdateInfo(
            Id: null, // exclude existing ID for cloning
            Name: item.Name,
            Description: item.Description,
            Price: price,
            Image: image,
            Ingredients: ingredients);
    }
}
