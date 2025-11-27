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
    IDateTimeProvider dateTimeProvider) : ICommandHandler<PublishMenuCommand, PublishMenuResponse>
{
    public async Task<PublishMenuResponse> HandleAsync(
        PublishMenuCommand request,
        CancellationToken cancellationToken = default)
    {
        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);

        if (menu == null || menu.CafeId != request.CafeId)
        {
            throw new InvalidOperationException("Menu not found");
        }

        if (menu.IsPublished)
        {
            throw new InvalidOperationException("Menu is already published");
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

        return new PublishMenuResponse(
            menu.Id,
            menu.Name,
            menu.PublishedAt.Value
        );
    }
}
