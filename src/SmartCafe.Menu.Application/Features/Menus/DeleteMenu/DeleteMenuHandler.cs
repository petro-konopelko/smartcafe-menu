using SmartCafe.Menu.Application.Features.Menus.DeleteMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.Application.Features.Menus.DeleteMenu;

public class DeleteMenuHandler(
    IMenuRepository menuRepository,
    IImageStorageService imageStorageService,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<DeleteMenuCommand, DeleteMenuResponse>
{
    public async Task<DeleteMenuResponse> HandleAsync(
        DeleteMenuCommand request,
        CancellationToken cancellationToken = default)
    {
        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        if (menu == null || menu.CafeId != request.CafeId)
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
        await imageStorageService.DeleteMenuImagesAsync(request.CafeId, request.MenuId, cancellationToken);

        var deletedEvent = new MenuDeletedEvent(
            Guid.CreateVersion7(),
            request.MenuId,
            request.CafeId,
            dateTimeProvider.UtcNow
        );
        await eventPublisher.PublishAsync(deletedEvent, cancellationToken);

        return new DeleteMenuResponse(true);
    }
}
