using SmartCafe.Menu.Application.Features.Menus.PublishMenu.Mappers;
using SmartCafe.Menu.Application.Features.Menus.PublishMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Shared.Providers.Abstractions;

namespace SmartCafe.Menu.Application.Features.Menus.PublishMenu;

public class PublishMenuHandler(
    IMenuRepository menuRepository,
    ICafeRepository cafeRepository,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<PublishMenuCommand, Result<PublishMenuResponse>>
{
    public async Task<Result<PublishMenuResponse>> HandleAsync(
        PublishMenuCommand request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Check cafe exists (not deleted)
        var cafeExists = await cafeRepository.ExistsActiveAsync(request.CafeId, cancellationToken);
        if (!cafeExists)
        {
            return Result<PublishMenuResponse>.Failure(Error.NotFound(
                $"Cafe with ID {request.CafeId} not found",
                CafeErrorCodes.CafeNotFound));
        }

        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        if (menu is null || menu.CafeId != request.CafeId)
        {
            return Result<PublishMenuResponse>.Failure(Error.NotFound(
                $"Menu with ID {request.MenuId} not found",
                MenuErrorCodes.MenuNotFound));
        }

        var publish = menu.Publish(dateTimeProvider);
        if (publish.IsFailure)
        {
            return Result<PublishMenuResponse>.Failure(publish.EnsureError());
        }

        await menuRepository.UpdateAsync(menu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var events = menu.DomainEvents.ToList();
        menu.ClearDomainEvents();
        await eventDispatcher.DispatchAsync(events, cancellationToken);

        return Result<PublishMenuResponse>.Success(menu.ToPublishMenuResponse());
    }
}
