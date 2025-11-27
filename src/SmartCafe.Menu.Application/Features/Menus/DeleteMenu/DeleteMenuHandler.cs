using SmartCafe.Menu.Application.Common.Results;
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
    IDateTimeProvider dateTimeProvider) : ICommandHandler<DeleteMenuCommand, Result>
{
    public async Task<Result> HandleAsync(
        DeleteMenuCommand request,
        CancellationToken cancellationToken = default)
    {
        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        if (menu == null || menu.CafeId != request.CafeId)
        {
            return Result.Failure(Error.NotFound(
                $"Menu with ID {request.MenuId} not found",
                "MENU_NOT_FOUND"));
        }

        if (menu.IsPublished)
        {
            return Result.Failure(Error.Conflict(
                "Cannot delete a published menu. Only draft menus can be deleted.",
                "MENU_PUBLISHED"));
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

        return Result.Success();
    }
}
