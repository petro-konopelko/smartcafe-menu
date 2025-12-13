using Microsoft.AspNetCore.Mvc;
using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.API.Models.Requests;
using SmartCafe.Menu.Application.Features.Menus.CreateMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class CreateMenuEndpoint
{
    public static RouteGroupBuilder MapCreateMenu(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            Guid cafeId,
            [FromBody] CreateMenuRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new CreateMenuCommand(cafeId, request.Name, request.Sections);
            var result = await mediator.Send<CreateMenuCommand, Result<CreateMenuResponse>>(command, ct);
            return result.ToCreatedResult(response => MenuRoutes.GetMenuLocation(cafeId, response.MenuId));
        })
        .WithName("CreateMenu")
        .WithSummary("Create a new menu with sections and items")
        .Produces<CreateMenuResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }
}
