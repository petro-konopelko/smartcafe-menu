using SmartCafe.Menu.Application.Features.Menus.ListMenus.Models;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class ListMenusEndpoint
{
    public static RouteGroupBuilder MapListMenus(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            Guid cafeId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new ListMenusQuery(cafeId);
            var result = await mediator.Send<ListMenusQuery, ListMenusResponse>(query, ct);
            return Results.Ok(result);
        })
        .WithName("ListMenus")
        .WithSummary("List all menus for a cafe")
        .Produces<ListMenusResponse>(StatusCodes.Status200OK);

        return group;
    }
}
