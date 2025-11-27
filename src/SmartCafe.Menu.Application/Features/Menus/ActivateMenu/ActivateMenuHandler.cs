using SmartCafe.Menu.Application.Common.Results;
using SmartCafe.Menu.Application.Features.Menus.ActivateMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.Application.Features.Menus.ActivateMenu;

public class ActivateMenuHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<ActivateMenuCommand, Result<ActivateMenuResponse>>
{
    public async Task<Result<ActivateMenuResponse>> HandleAsync(
        ActivateMenuCommand request,
        CancellationToken cancellationToken = default)
    {
        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        if (menu == null || menu.CafeId != request.CafeId)
        {
            return Result<ActivateMenuResponse>.Failure(Error.NotFound(
                $"Menu with ID {request.MenuId} not found",
                ErrorCodes.MenuNotFound));
        }

        if (!menu.IsPublished)
        {
            return Result<ActivateMenuResponse>.Failure(Error.Conflict(
                "Menu must be published before it can be activated",
                ErrorCodes.MenuNotPublished));
        }

        if (menu.IsActive)
        {
            return Result<ActivateMenuResponse>.Failure(Error.Conflict(
                "Menu is already active",
                ErrorCodes.MenuAlreadyActive));
        }

        // Deactivate currently active menu
        var currentActiveMenu = await menuRepository.GetActiveMenuAsync(request.CafeId, cancellationToken);
        if (currentActiveMenu != null)
        {
            currentActiveMenu.IsActive = false;
            await menuRepository.UpdateAsync(currentActiveMenu, cancellationToken);

            var deactivatedEvent = new MenuDeactivatedEvent(
                Guid.CreateVersion7(),
                currentActiveMenu.Id,
                request.CafeId,
                dateTimeProvider.UtcNow
            );
            await eventPublisher.PublishAsync(deactivatedEvent, cancellationToken);
        }

        // Activate new menu
        menu.IsActive = true;
        menu.ActivatedAt = dateTimeProvider.UtcNow;

        await menuRepository.UpdateAsync(menu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish activation event
        var activatedEvent = new MenuActivatedEvent(
            Guid.CreateVersion7(),
            menu.Id,
            request.CafeId,
            menu.Name,
            dateTimeProvider.UtcNow
        );
        await eventPublisher.PublishAsync(activatedEvent, cancellationToken);

        return Result<ActivateMenuResponse>.Success(new ActivateMenuResponse(
            menu.Id,
            menu.Name,
            menu.ActivatedAt.Value
        ));
    }
}
