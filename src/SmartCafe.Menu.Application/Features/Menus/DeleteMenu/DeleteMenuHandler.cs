using SmartCafe.Menu.Application.Features.Menus.DeleteMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain;
using SmartCafe.Menu.Domain.Common;
using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.Application.Features.Menus.DeleteMenu;

public class DeleteMenuHandler(
    IMenuRepository menuRepository,
    IImageStorageService imageStorageService,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<DeleteMenuCommand, Result>
{
    public async Task<Result> HandleAsync(
        DeleteMenuCommand request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        if (menu == null || menu.CafeId != request.CafeId)
        {
            return Result.Failure(Error.NotFound(
                $"Menu with ID {request.MenuId} not found",
                ErrorCodes.MenuNotFound));
        }

        var deletion = menu.SoftDelete(dateTimeProvider);
        if (deletion.IsFailure)
            return Result.Failure(deletion.Error!);

        await menuRepository.UpdateAsync(menu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Delete all menu images from storage
        await imageStorageService.DeleteMenuImagesAsync(request.CafeId, request.MenuId, cancellationToken);

        var events = menu.DomainEvents.ToList();
        menu.ClearDomainEvents();
        await eventDispatcher.DispatchAsync(events, cancellationToken);

        return Result.Success();
    }
}
