using Microsoft.AspNetCore.Mvc;
using SmartCafe.Menu.Application.Features.Menus.CreateMenu.Models;
using SmartCafe.Menu.Application.Mediation.Core;

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
            try
            {
                var command = request with { CafeId = cafeId };
                var response = await mediator.Send<CreateMenuRequest, CreateMenuResponse>(command, ct);
                return Results.Created($"/api/cafes/{cafeId}/menus/{response.Id}", response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
        })
        .WithName("CreateMenu")
        .WithSummary("Create a new menu with sections and items")
        .Produces<CreateMenuResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status400BadRequest);

        return group;
    }
}
