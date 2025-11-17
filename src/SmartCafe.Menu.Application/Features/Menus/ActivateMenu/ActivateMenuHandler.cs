using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.Application.Features.Menus.ActivateMenu;

public class ActivateMenuHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    IDateTimeProvider dateTimeProvider)
{
    public async Task<ActivateMenuResponse> HandleAsync(
        Guid cafeId,
        Guid menuId,
        CancellationToken cancellationToken = default)
    {
        var menu = await menuRepository.GetByIdAsync(menuId, cancellationToken);

        if (menu == null || menu.CafeId != cafeId)
        {
            throw new InvalidOperationException("Menu not found");
        }

        if (!menu.IsPublished)
        {
            throw new InvalidOperationException("Menu must be published before it can be activated");
        }

        if (menu.IsActive)
        {
            throw new InvalidOperationException("Menu is already active");
        }

        // Deactivate currently active menu
        var currentActiveMenu = await menuRepository.GetActiveMenuAsync(cafeId, cancellationToken);
        if (currentActiveMenu != null)
        {
            currentActiveMenu.IsActive = false;
            await menuRepository.UpdateAsync(currentActiveMenu, cancellationToken);

            var deactivatedEvent = new MenuDeactivatedEvent(
                Guid.CreateVersion7(),
                currentActiveMenu.Id,
                cafeId,
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
            cafeId,
            menu.Name,
            dateTimeProvider.UtcNow
        );
        await eventPublisher.PublishAsync(activatedEvent, cancellationToken);

        return new ActivateMenuResponse(
            menu.Id,
            menu.Name,
            menu.ActivatedAt.Value
        );
    }
}
