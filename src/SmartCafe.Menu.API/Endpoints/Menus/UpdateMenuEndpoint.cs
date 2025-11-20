using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCafe.Menu.Application.Features.Menus.UpdateMenu;
using SmartCafe.Menu.Domain.Exceptions;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class UpdateMenuEndpoint
{
    public static RouteGroupBuilder MapUpdateMenu(this RouteGroupBuilder group)
    {
        group.MapPut("/{menuId:guid}", async (
            Guid cafeId,
            Guid menuId,
            [FromBody] UpdateMenuRequest request,
            IValidator<UpdateMenuRequest> validator,
            UpdateMenuHandler handler,
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
                return Results.Ok(response);
            }
            catch (MenuNotFoundException)
            {
                return Results.NotFound(new { Message = $"Menu with ID {menuId} not found" });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
        })
        .WithName("UpdateMenu")
        .WithSummary("Update an existing menu with sections and items")
        .Produces<UpdateMenuResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status400BadRequest);

        return group;
    }
}
