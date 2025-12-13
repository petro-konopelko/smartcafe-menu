using SmartCafe.Menu.Application.Features.Menus.Shared.Mappers;
using SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Shared.Providers.Abstractions;

namespace SmartCafe.Menu.Application.Features.Menus.UpdateMenu;

public class UpdateMenuHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher,
    IDateTimeProvider dateTimeProvider,
    IGuidIdProvider guidIdProvider) : ICommandHandler<UpdateMenuCommand, Result>
{
    public async Task<Result> HandleAsync(
        UpdateMenuCommand request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Load existing menu
        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);
        if (menu is null || menu.CafeId != request.CafeId)
        {
            return Result.Failure(Error.NotFound(
                $"Menu with ID {request.MenuId} not found",
                MenuErrorCodes.MenuNotFound));
        }

        // Update menu via domain
        var updateResult = menu.SyncMenu(
            request.Name,
            request.Sections.ToSectionUpdateInfoCollection(),
            dateTimeProvider,
            guidIdProvider);

        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.EnsureError());
        }

        // Persist changes
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Dispatch domain events directly from menu
        var events = menu.DomainEvents.ToList();
        menu.ClearDomainEvents();
        await eventDispatcher.DispatchAsync(events, cancellationToken);

        return Result.Success();
    }
}
