using SmartCafe.Menu.Application.Features.Categories.ListCategories;

namespace SmartCafe.Menu.API.Endpoints.Categories;

public static class ListCategoriesEndpoint
{
    public static RouteGroupBuilder MapListCategories(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            ListCategoriesHandler handler,
            CancellationToken ct) =>
        {
            var result = await handler.HandleAsync(ct);
            return Results.Ok(result);
        })
        .WithName("ListCategories")
        .WithSummary("Get all categories")
        .Produces<List<CategoryDto>>(StatusCodes.Status200OK);

        return group;
    }
}
