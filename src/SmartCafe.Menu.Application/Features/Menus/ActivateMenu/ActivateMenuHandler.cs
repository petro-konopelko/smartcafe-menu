using SmartCafe.Menu.Application.Features.Menus.ActivateMenu.Mappers;
using SmartCafe.Menu.Application.Features.Menus.ActivateMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Shared.Providers.Abstractions;

namespace SmartCafe.Menu.Application.Features.Menus.ActivateMenu;

public class ActivateMenuHandler(
    IMenuRepository menuRepository,
    ICafeRepository cafeRepository,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<ActivateMenuCommand, Result<ActivateMenuResponse>>
{
    public async Task<Result<ActivateMenuResponse>> HandleAsync(
        ActivateMenuCommand request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Check cafe exists (not deleted)
        var cafeExists = await cafeRepository.ExistsActiveAsync(request.CafeId, cancellationToken);
        if (!cafeExists)
        {
            return Result<ActivateMenuResponse>.Failure(Error.NotFound(
                $"Cafe with ID {request.CafeId} not found",
                CafeErrorCodes.CafeNotFound));
        }

        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        if (menu is null || menu.CafeId != request.CafeId)
        {
            return Result<ActivateMenuResponse>.Failure(Error.NotFound(
                $"Menu with ID {request.MenuId} not found",
                MenuErrorCodes.MenuNotFound));
        }

        // Deactivate currently active menu via domain
        var currentActiveMenu = await menuRepository.GetActiveMenuAsync(request.CafeId, cancellationToken);
        if (currentActiveMenu is not null)
        {
            var deactivation = currentActiveMenu.Deactivate(dateTimeProvider);
            if (deactivation.IsFailure)
            {
                return Result<ActivateMenuResponse>.Failure(deactivation.EnsureError());
            }

            await menuRepository.UpdateAsync(currentActiveMenu, cancellationToken);
        }

        // Activate new menu via domain
        var activation = menu.Activate(dateTimeProvider);
        if (activation.IsFailure)
        {
            return Result<ActivateMenuResponse>.Failure(activation.EnsureError());
        }

        await menuRepository.UpdateAsync(menu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var events = new List<DomainEvent>();
        if (currentActiveMenu is not null)
        {
            events.AddRange(currentActiveMenu.DomainEvents);
            currentActiveMenu.ClearDomainEvents();
        }
        events.AddRange(menu.DomainEvents);
        menu.ClearDomainEvents();
        await eventDispatcher.DispatchAsync(events, cancellationToken);

        return Result<ActivateMenuResponse>.Success(menu.ToActivateMenuResponse());
    }
}
