using SmartCafe.Menu.Application.Features.Menus.CloneMenu.Mappers;
using SmartCafe.Menu.Application.Features.Menus.CloneMenu.Models;
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
    IDateTimeProvider dateTimeProvider) : ICommandHandler<CloneMenuRequest, Result<CloneMenuResponse>>
{
    public async Task<Result<CloneMenuResponse>> HandleAsync(
        CloneMenuRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Load source menu
        var sourceMenu = await menuRepository.GetByIdAsync(request.SourceMenuId, cancellationToken);
        if (sourceMenu == null || sourceMenu.CafeId != request.CafeId)
        {
            return Result<CloneMenuResponse>.Failure(Error.NotFound(
                $"Menu with ID {request.SourceMenuId} not found",
                ErrorCodes.MenuNotFound));
        }

        // Create cloned menu via domain factory
        var clonedMenuResult = Domain.Entities.Menu.CloneFrom(sourceMenu, request.NewMenuName, dateTimeProvider);
        if (clonedMenuResult.IsFailure)
            return Result<CloneMenuResponse>.Failure(clonedMenuResult.Error!);

        // Save cloned menu
        await menuRepository.CreateAsync(clonedMenuResult.Value!, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Dispatch domain events directly from cloned menu
        var events = clonedMenuResult.Value!.DomainEvents.ToList();
        clonedMenuResult.Value!.ClearDomainEvents();
        await eventDispatcher.DispatchAsync(events, cancellationToken);

        return Result<CloneMenuResponse>.Success(clonedMenuResult.Value!.ToCloneMenuResponse());
    }
}
