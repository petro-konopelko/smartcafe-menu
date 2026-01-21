using SmartCafe.Menu.Application.Features.Cafes.GetCafe.Models;
using SmartCafe.Menu.Application.Features.Cafes.Shared.Mappers;
using SmartCafe.Menu.Application.Features.Cafes.Shared.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Cafes.GetCafe;

public class GetCafeHandler(ICafeRepository cafeRepository) : IQueryHandler<GetCafeQuery, Result<CafeDto>>
{
    public async Task<Result<CafeDto>> HandleAsync(
        GetCafeQuery request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var cafe = await cafeRepository.GetActiveByIdAsync(request.CafeId, cancellationToken);

        return cafe is null
            ? Result<CafeDto>.Failure(Error.NotFound(
                $"Cafe with ID {request.CafeId} not found",
                CafeErrorCodes.CafeNotFound))
            : Result<CafeDto>.Success(cafe.ToCafeDto());
    }
}
