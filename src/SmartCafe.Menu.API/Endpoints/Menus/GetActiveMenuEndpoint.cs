using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.Application.Common.Results;
using SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Models;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class GetActiveMenuEndpoint
{
    public static RouteGroupBuilder MapGetActiveMenu(this RouteGroupBuilder group)
    {
        group.MapGet("/active", async (
            Guid cafeId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var query = new GetActiveMenuQuery(cafeId);
            var result = await mediator.Send<GetActiveMenuQuery, Result<GetActiveMenuResponse>>(query, ct);
            return result.ToApiResult();
        })
        .WithName("GetActiveMenu")
        .WithSummary("Get the currently active menu for customers")
        .Produces<GetActiveMenuResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
