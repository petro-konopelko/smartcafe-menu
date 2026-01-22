using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.Application.Features.Cafes.ListCafes.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.API.Endpoints.Cafes;

public static class ListCafesEndpoint
{
    public static RouteGroupBuilder MapListCafes(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new ListCafesQuery();
            var result = await mediator.Send<ListCafesQuery, Result<ListCafesResponse>>(query, ct);
            return result.ToApiResult();
        })
        .WithName("ListCafes")
        .WithSummary("List all active cafes")
        .Produces<ListCafesResponse>(StatusCodes.Status200OK);

        return group;
    }
}
