using SmartCafe.Menu.Application.Features.Menus.ActivateMenu;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class ActivateMenuEndpoint
{
    public static RouteGroupBuilder MapActivateMenu(this RouteGroupBuilder group)
    {
        group.MapPost("/{menuId:guid}/activate", async (
            Guid cafeId,
            Guid menuId,
            ActivateMenuHandler handler,
            CancellationToken ct) =>
        {
            try
            {
                var response = await handler.HandleAsync(cafeId, menuId, ct);
                return Results.Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
        })
        .WithName("ActivateMenu")
        .WithSummary("Activate a published menu (deactivates current active menu)")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
