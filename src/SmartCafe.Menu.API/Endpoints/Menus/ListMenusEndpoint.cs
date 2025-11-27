using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.Application.Features.Menus.ListMenus.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Common;

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
            var result = await mediator.Send<ListMenusQuery, Result<ListMenusResponse>>(query, ct);
            return result.ToApiResult();
        })
        .WithName("ListMenus")
        .WithSummary("List all menus for a cafe")
        .Produces<ListMenusResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }
}
