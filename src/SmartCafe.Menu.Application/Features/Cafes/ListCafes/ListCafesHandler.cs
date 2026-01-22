using SmartCafe.Menu.Application.Features.Cafes.ListCafes.Models;
using SmartCafe.Menu.Application.Features.Cafes.Shared.Mappers;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Cafes.ListCafes;

public class ListCafesHandler(
    ICafeRepository cafeRepository) : IQueryHandler<ListCafesQuery, Result<ListCafesResponse>>
{
    public async Task<Result<ListCafesResponse>> HandleAsync(
        ListCafesQuery request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var cafes = await cafeRepository.GetAllActiveAsync(cancellationToken);

        return Result<ListCafesResponse>.Success(new ListCafesResponse(
            [.. cafes.Select(c => c.ToCafeDto())]
        ));
    }
}
