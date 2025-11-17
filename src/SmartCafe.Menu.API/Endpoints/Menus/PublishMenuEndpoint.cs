using SmartCafe.Menu.Application.Features.Menus.PublishMenu;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class PublishMenuEndpoint
{
    public static RouteGroupBuilder MapPublishMenu(this RouteGroupBuilder group)
    {
        group.MapPost("/{menuId:guid}/publish", async (
            Guid cafeId,
            Guid menuId,
            PublishMenuHandler handler,
            CancellationToken ct) =>
        {
            try
            {
                var result = await handler.HandleAsync(cafeId, menuId, ct);
                return Results.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithName("PublishMenu")
        .WithSummary("Publish a menu (makes it available for activation)")
        .Produces<PublishMenuResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
