using Microsoft.AspNetCore.Mvc;
using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.Application.Features.Menus.CloneMenu.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class CloneMenuEndpoint
{
    public static RouteGroupBuilder MapCloneMenu(this RouteGroupBuilder group)
    {
        group.MapPost("/{menuId:guid}/clone", async (
            Guid cafeId,
            Guid menuId,
            [FromBody] CloneMenuRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = request with { CafeId = cafeId, SourceMenuId = menuId };
            var result = await mediator.Send<CloneMenuRequest, Result<CloneMenuResponse>>(command, ct);
            return result.ToCreatedResult(response => MenuRoutes.GetMenuLocation(cafeId, response.Id));
        })
        .WithName("CloneMenu")
        .WithSummary("Clone an existing menu to create a new draft menu")
        .Produces<CloneMenuResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }
}
