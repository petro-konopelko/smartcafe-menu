using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.Application.Features.Menus.DeleteMenu.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class DeleteMenuEndpoint
{
    public static RouteGroupBuilder MapDeleteMenu(this RouteGroupBuilder group)
    {
        group.MapDelete("/{menuId:guid}", async (
            Guid cafeId,
            Guid menuId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new DeleteMenuCommand(cafeId, menuId);
            var result = await mediator.Send<DeleteMenuCommand, Result>(command, ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteMenu")
        .WithSummary("Delete a new menu (published menus cannot be deleted)")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }
}
