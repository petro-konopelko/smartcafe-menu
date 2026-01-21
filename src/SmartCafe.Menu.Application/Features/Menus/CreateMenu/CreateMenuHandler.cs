using SmartCafe.Menu.Application.Features.Menus.CreateMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Mappers;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Shared.Providers.Abstractions;
using MenuEntity = SmartCafe.Menu.Domain.Entities.Menu;

namespace SmartCafe.Menu.Application.Features.Menus.CreateMenu;

public class CreateMenuHandler(
    IMenuRepository menuRepository,
    ICafeRepository cafeRepository,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher,
    IDateTimeProvider dateTimeProvider,
    IGuidIdProvider guidIdProvider) : ICommandHandler<CreateMenuCommand, Result<CreateMenuResponse>>
{
    public async Task<Result<CreateMenuResponse>> HandleAsync(
        CreateMenuCommand request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Check cafe exists
        var cafeExists = await cafeRepository.ExistsActiveAsync(request.CafeId, cancellationToken);
        if (!cafeExists)
        {
            return Result<CreateMenuResponse>.Failure(Error.NotFound(
                $"Cafe with ID {request.CafeId} not found",
                CafeErrorCodes.CafeNotFound));
        }

        // Create menu using factory method
        var menuResult = MenuEntity.Create(
            request.CafeId,
            request.Name,
            guidIdProvider,
            dateTimeProvider,
            request.Sections.ToSectionUpdateInfoCollection());

        if (menuResult.IsFailure)
        {
            return Result<CreateMenuResponse>.Failure(menuResult.EnsureError());
        }

        var menu = menuResult.EnsureValue();

        // Save to database
        await menuRepository.CreateAsync(menu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Dispatch domain events
        var events = menu.DomainEvents;
        menu.ClearDomainEvents();
        await eventDispatcher.DispatchAsync(events, cancellationToken);

        return Result<CreateMenuResponse>.Success(menu.ToCreateMenuResponse());
    }
}
