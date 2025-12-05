using SmartCafe.Menu.Application.Features.Menus.PublishMenu.Mappers;
using SmartCafe.Menu.Application.Features.Menus.PublishMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain;
using SmartCafe.Menu.Domain.Common;
using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.Application.Features.Menus.PublishMenu;

public class PublishMenuHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<PublishMenuCommand, Result<PublishMenuResponse>>
{
    public async Task<Result<PublishMenuResponse>> HandleAsync(
        PublishMenuCommand request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        if (menu is null || menu.CafeId != request.CafeId)
        {
            return Result<PublishMenuResponse>.Failure(Error.NotFound(
                $"Menu with ID {request.MenuId} not found",
                ErrorCodes.MenuNotFound));
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
