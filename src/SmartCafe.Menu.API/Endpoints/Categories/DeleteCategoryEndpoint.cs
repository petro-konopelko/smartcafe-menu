using SmartCafe.Menu.Application.Features.Categories.DeleteCategory;

namespace SmartCafe.Menu.API.Endpoints.Categories;

public static class DeleteCategoryEndpoint
{
    public static RouteGroupBuilder MapDeleteCategory(this RouteGroupBuilder group)
    {
        group.MapDelete("/{categoryId:guid}", async (
            Guid categoryId,
            DeleteCategoryHandler handler,
            CancellationToken ct) =>
        {
            try
            {
                await handler.HandleAsync(categoryId, ct);
                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithName("DeleteCategory")
        .WithSummary("Delete a custom category")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
