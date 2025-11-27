using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.Application.Common.Results;
using SmartCafe.Menu.Application.Features.Menus.ActivateMenu.Models;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class ActivateMenuEndpoint
{
    public static RouteGroupBuilder MapActivateMenu(this RouteGroupBuilder group)
    {
        group.MapPost("/{menuId:guid}/activate", async (
            Guid cafeId,
            Guid menuId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new ActivateMenuCommand(cafeId, menuId);
            var result = await mediator.Send<ActivateMenuCommand, Result<ActivateMenuResponse>>(command, ct);
            return result.ToApiResult();
        })
        .WithName("ActivateMenu")
        .WithSummary("Activate a published menu (deactivates current active menu)")
        .Produces(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }
}
