using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCafe.Menu.Application.Features.Menus.UpsertMenu;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class UpsertMenuEndpoint
{
    public static RouteGroupBuilder MapUpsertMenu(this RouteGroupBuilder group)
    {
        group.MapPost("/upsert", async (
            Guid cafeId,
            [FromBody] UpsertMenuRequest request,
            IValidator<UpsertMenuRequest> validator,
            UpsertMenuHandler handler,
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
                var response = await handler.HandleAsync(cafeId, request, ct);
                var isUpdate = request.MenuId.HasValue;

                return isUpdate
                    ? Results.Ok(response)
                    : Results.Created($"/api/cafes/{cafeId}/menus/{response.MenuId}", response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { Message = ex.Message });
            }
        })
        .WithName("UpsertMenu")
        .WithSummary("Create or update a complete menu with sections and items")
        .Produces<UpsertMenuResponse>(StatusCodes.Status200OK)
        .Produces<UpsertMenuResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status400BadRequest);

        return group;
    }
}
