using SmartCafe.Menu.Application.Features.Menus.ListMenus;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class ListMenusEndpoint
{
    public static RouteGroupBuilder MapListMenus(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            Guid cafeId,
            ListMenusHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(cafeId, ct);
            return Results.Ok(result);
        })
        .WithName("ListMenus")
        .WithSummary("List all menus for a cafe")
        .Produces<List<MenuSummaryDto>>(StatusCodes.Status200OK);

        return group;
    }
}
