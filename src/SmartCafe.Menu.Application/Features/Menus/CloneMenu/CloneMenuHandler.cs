using SmartCafe.Menu.Application.Features.Menus.CloneMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Mappers;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain;
using SmartCafe.Menu.Domain.Common;
using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.Application.Features.Menus.CloneMenu;

public class CloneMenuHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<CloneMenuCommand, Result<CreateMenuResponse>>
{
    public async Task<Result<CreateMenuResponse>> HandleAsync(
        CloneMenuCommand request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Load source menu
        var sourceMenu = await menuRepository.GetByIdAsync(request.SourceMenuId, cancellationToken);
        if (sourceMenu is null || sourceMenu.CafeId != request.CafeId)
        {
            return Result<CreateMenuResponse>.Failure(Error.NotFound(
                $"Menu with ID {request.SourceMenuId} not found",
                ErrorCodes.MenuNotFound));
        }

        // Create cloned menu via domain factory
        var clonedMenuResult = Domain.Entities.Menu.CloneFrom(sourceMenu, request.NewName, dateTimeProvider);

        if (clonedMenuResult.IsFailure)
            return Result<CreateMenuResponse>.Failure(clonedMenuResult.EnsureError());

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
}
