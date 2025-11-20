using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCafe.Menu.Application.Features.Menus.CloneMenu;
using SmartCafe.Menu.Domain.Exceptions;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class CloneMenuEndpoint
{
    public static RouteGroupBuilder MapCloneMenu(this RouteGroupBuilder group)
    {
        group.MapPost("/{menuId:guid}/clone", async (
            Guid cafeId,
            Guid menuId,
            [FromBody] CloneMenuRequest request,
            IValidator<CloneMenuRequest> validator,
            CloneMenuHandler handler,
            CancellationToken ct) =>
        {
            // Validate request
            var validationResult = await validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            try
            {
                var response = await handler.HandleAsync(cafeId, menuId, request, ct);
                return Results.Created($"/api/cafes/{cafeId}/menus/{response.Id}", response);
            }
            catch (MenuNotFoundException)
            {
                return Results.NotFound(new { Message = $"Source menu with ID {menuId} not found" });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
        })
        .WithName("CloneMenu")
        .WithSummary("Clone an existing menu to create a new draft menu")
        .Produces<CloneMenuResponse>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status400BadRequest);

        return group;
    }
}
