using SmartCafe.Menu.Application.Features.Menus.DeleteMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Shared.Providers.Abstractions;

namespace SmartCafe.Menu.Application.Features.Menus.DeleteMenu;

public class DeleteMenuHandler(
    IMenuRepository menuRepository,
    ICafeRepository cafeRepository,
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

        // Check cafe exists (not deleted)
        var cafeExists = await cafeRepository.ExistsActiveAsync(request.CafeId, cancellationToken);
        if (!cafeExists)
        {
            return Result.Failure(Error.NotFound(
                $"Cafe with ID {request.CafeId} not found",
                CafeErrorCodes.CafeNotFound));
        }

        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        if (menu is null || menu.CafeId != request.CafeId)
        {
            return Result.Failure(Error.NotFound(
                $"Menu with ID {request.MenuId} not found",
                MenuErrorCodes.MenuNotFound));
        }

        var deletion = menu.SoftDelete(dateTimeProvider);
        if (deletion.IsFailure)
        {
            return Result.Failure(deletion.EnsureError());
        }

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
