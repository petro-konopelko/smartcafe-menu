using SmartCafe.Menu.Application.Features.Cafes.CreateCafe.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Shared.Providers.Abstractions;
using CafeEntity = SmartCafe.Menu.Domain.Entities.Cafe;

namespace SmartCafe.Menu.Application.Features.Cafes.CreateCafe;

public class CreateCafeHandler(
    ICafeRepository cafeRepository,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher,
    IDateTimeProvider dateTimeProvider,
    IGuidIdProvider guidIdProvider) : ICommandHandler<CreateCafeCommand, Result<CreateCafeResponse>>
{
    public async Task<Result<CreateCafeResponse>> HandleAsync(
        CreateCafeCommand request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var cafeId = guidIdProvider.NewId();

        // Create cafe using factory method
        var cafeResult = CafeEntity.Create(
            cafeId,
            request.Name,
            dateTimeProvider,
            request.ContactInfo);

        if (cafeResult.IsFailure)
        {
            return Result<CreateCafeResponse>.Failure(cafeResult.EnsureError());
        }

        var cafe = cafeResult.EnsureValue();

        // Persist cafe
        await cafeRepository.CreateAsync(cafe, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Dispatch domain events
        var events = cafe.DomainEvents.ToList();
        cafe.ClearDomainEvents();
        await eventDispatcher.DispatchAsync(events, cancellationToken);

        return Result<CreateCafeResponse>.Success(new CreateCafeResponse(cafe.Id));
    }
}
