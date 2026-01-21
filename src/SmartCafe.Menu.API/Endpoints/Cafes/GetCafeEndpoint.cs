using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.Application.Features.Cafes.GetCafe.Models;
using SmartCafe.Menu.Application.Features.Cafes.Shared.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.API.Endpoints.Cafes;

public static class GetCafeEndpoint
{
    public static RouteGroupBuilder MapGetCafe(this RouteGroupBuilder group)
    {
        group.MapGet("/{cafeId:guid}", async (
            Guid cafeId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new GetCafeQuery(cafeId);
            var result = await mediator.Send<GetCafeQuery, Result<CafeDto>>(query, ct);
            return result.ToApiResult();
        })
        .WithName("GetCafe")
        .WithSummary("Get cafe details by ID")
        .Produces<CafeDto>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }
}
