using SmartCafe.Menu.Application.Features.Menus.GetMenu;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class GetMenuEndpoint
{
    public static RouteGroupBuilder MapGetMenu(this RouteGroupBuilder group)
    {
        group.MapGet("/{menuId:guid}", async (
            Guid cafeId,
            Guid menuId,
            GetMenuHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(cafeId, menuId, ct);
            
            if (result == null)
            {
                return Results.NotFound(new { message = "Menu not found" });
            }

            return Results.Ok(result);
        })
        .WithName("GetMenu")
        .WithSummary("Get a menu with all its sections and items")
        .Produces<GetMenuResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
