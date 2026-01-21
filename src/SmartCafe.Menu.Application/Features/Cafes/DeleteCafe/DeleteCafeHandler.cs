using SmartCafe.Menu.Application.Features.Cafes.DeleteCafe.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Shared.Providers.Abstractions;

namespace SmartCafe.Menu.Application.Features.Cafes.DeleteCafe;

public class DeleteCafeHandler(
    ICafeRepository cafeRepository,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<DeleteCafeCommand, Result>
{
    public async Task<Result> HandleAsync(
        DeleteCafeCommand request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var cafe = await cafeRepository.GetActiveByIdAsync(request.CafeId, cancellationToken);

        if (cafe is null)
        {
            return Result.Failure(Error.NotFound(
                $"Cafe with ID {request.CafeId} not found",
                CafeErrorCodes.CafeNotFound));
        }

        var deletion = cafe.SoftDelete(dateTimeProvider);
        if (deletion.IsFailure)
        {
            return Result.Failure(deletion.EnsureError());
        }

        await cafeRepository.UpdateAsync(cafe, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var events = cafe.DomainEvents.ToList();
        cafe.ClearDomainEvents();
        await eventDispatcher.DispatchAsync(events, cancellationToken);

        return Result.Success();
    }
}
