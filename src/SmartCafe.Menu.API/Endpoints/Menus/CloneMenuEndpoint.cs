using Microsoft.AspNetCore.Mvc;
using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.API.Models.Requests;
using SmartCafe.Menu.Application.Features.Menus.CloneMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

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
            var command = new CloneMenuCommand(cafeId, menuId, request.NewName);
            var result = await mediator.Send<CloneMenuCommand, Result<CreateMenuResponse>>(command, ct);
            return result.ToCreatedResult(response => MenuRoutes.GetMenuLocation(cafeId, response.MenuId));
        })
        .WithName("CloneMenu")
        .WithSummary("Clone an existing menu with a new name")
        .Produces<CreateMenuResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }
}
