using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.Application.Features.Menus.DeleteMenu;

public class DeleteMenuHandler(
    IMenuRepository menuRepository,
    IImageStorageService imageStorageService,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    IDateTimeProvider dateTimeProvider)
{
    public async Task HandleAsync(
        Guid cafeId,
        Guid menuId,
        CancellationToken cancellationToken = default)
    {
        var menu = await menuRepository.GetByIdAsync(menuId, cancellationToken);

        if (menu == null || menu.CafeId != cafeId)
        {
            throw new InvalidOperationException("Menu not found");
        }

        if (menu.IsPublished)
        {
            throw new InvalidOperationException("Cannot delete a published menu. Only draft menus can be deleted.");
        }

        await menuRepository.DeleteAsync(menu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Delete all menu images from storage
        await imageStorageService.DeleteMenuImagesAsync(cafeId, menuId, cancellationToken);

        var deletedEvent = new MenuDeletedEvent(
            Guid.CreateVersion7(),
            menuId,
            cafeId,
            dateTimeProvider.UtcNow
        );
        await eventPublisher.PublishAsync(deletedEvent, cancellationToken);
    }
}
