using SmartCafe.Menu.Application.Features.Menus.DeleteMenu;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class DeleteMenuEndpoint
{
    public static RouteGroupBuilder MapDeleteMenu(this RouteGroupBuilder group)
    {
        group.MapDelete("/{menuId:guid}", async (
            Guid cafeId,
            Guid menuId,
            DeleteMenuHandler handler,
            CancellationToken ct) =>
        {
            try
            {
                await handler.HandleAsync(cafeId, menuId, ct);
                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithName("DeleteMenu")
        .WithSummary("Delete a draft menu (published menus cannot be deleted)")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
