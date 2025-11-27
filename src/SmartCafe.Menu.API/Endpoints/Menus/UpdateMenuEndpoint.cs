using Microsoft.AspNetCore.Mvc;
using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.Application.Common.Results;
using SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Models;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class UpdateMenuEndpoint
{
    public static RouteGroupBuilder MapUpdateMenu(this RouteGroupBuilder group)
    {
        group.MapPut("/{menuId:guid}", async (
            Guid cafeId,
            Guid menuId,
            [FromBody] UpdateMenuRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = request with { CafeId = cafeId, MenuId = menuId };
            var result = await mediator.Send<UpdateMenuRequest, Result<UpdateMenuResponse>>(command, ct);
            return result.ToApiResult();
        })
        .WithName("UpdateMenu")
        .WithSummary("Update an existing menu with sections and items")
        .Produces<UpdateMenuResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }
}
