using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.Application.Common.Results;
using SmartCafe.Menu.Application.Features.Menus.GetMenu.Models;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class GetMenuEndpoint
{
    public static RouteGroupBuilder MapGetMenu(this RouteGroupBuilder group)
    {
        group.MapGet("/{menuId:guid}", async (
            Guid cafeId,
            Guid menuId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new GetMenuQuery(cafeId, menuId);
            var result = await mediator.Send<GetMenuQuery, Result<GetMenuResponse>>(query, ct);
            return result.ToApiResult();
        })
        .WithName("GetMenu")
        .WithSummary("Get a menu with all its sections and items")
        .Produces<GetMenuResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }
}
