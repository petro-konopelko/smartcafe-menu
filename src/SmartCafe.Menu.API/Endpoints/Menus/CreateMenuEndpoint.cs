using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCafe.Menu.Application.Features.Menus.CreateMenu;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class CreateMenuEndpoint
{
    public static RouteGroupBuilder MapCreateMenu(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            Guid cafeId,
            [FromBody] CreateMenuRequest request,
            IValidator<CreateMenuRequest> validator,
            CreateMenuHandler handler,
            CancellationToken ct) =>
        {
            var validationResult = await validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var response = await handler.HandleAsync(cafeId, request, ct);
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
