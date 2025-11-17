using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.Application.Features.Menus.PublishMenu;

public class PublishMenuHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    IDateTimeProvider dateTimeProvider)
{
    public async Task<PublishMenuResponse> HandleAsync(
        Guid cafeId,
        Guid menuId,
        CancellationToken cancellationToken = default)
    {
        var menu = await menuRepository.GetByIdAsync(menuId, cancellationToken);

        if (menu == null || menu.CafeId != cafeId)
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
            cafeId,
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
