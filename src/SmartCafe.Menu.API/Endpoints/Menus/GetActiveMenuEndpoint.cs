using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Common;

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
            var result = await mediator.Send<GetActiveMenuQuery, Result<MenuDto>>(query, ct);
            return result.ToApiResult();
        })
        .WithName("GetActiveMenu")
        .WithSummary("Get the currently active menu for a cafe")
        .Produces<MenuDto>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }
}
