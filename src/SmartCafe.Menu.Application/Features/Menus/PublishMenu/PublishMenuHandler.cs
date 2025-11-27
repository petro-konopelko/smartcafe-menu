using SmartCafe.Menu.Application.Common.Results;
using SmartCafe.Menu.Application.Features.Menus.PublishMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.Application.Features.Menus.PublishMenu;

public class PublishMenuHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<PublishMenuCommand, Result<PublishMenuResponse>>
{
    public async Task<Result<PublishMenuResponse>> HandleAsync(
        PublishMenuCommand request,
        CancellationToken cancellationToken = default)
    {
        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        if (menu == null || menu.CafeId != request.CafeId)
        {
            return Result<PublishMenuResponse>.Failure(Error.NotFound(
                $"Menu with ID {request.MenuId} not found",
                "MENU_NOT_FOUND"));
        }

        if (menu.IsPublished)
        {
            return Result<PublishMenuResponse>.Failure(Error.Conflict(
                "Menu is already published",
                "MENU_ALREADY_PUBLISHED"));
        }

        menu.IsPublished = true;
        menu.PublishedAt = dateTimeProvider.UtcNow;

        await menuRepository.UpdateAsync(menu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var publishedEvent = new MenuPublishedEvent(
            Guid.CreateVersion7(),
            menu.Id,
            request.CafeId,
            menu.Name,
            dateTimeProvider.UtcNow
        );
        await eventPublisher.PublishAsync(publishedEvent, cancellationToken);

        return Result<PublishMenuResponse>.Success(new PublishMenuResponse(
            menu.Id,
            menu.Name,
            menu.PublishedAt.Value
        ));
    }
}
