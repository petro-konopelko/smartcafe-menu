using Microsoft.AspNetCore.Mvc;
using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.API.Models.Requests.Cafes;
using SmartCafe.Menu.Application.Features.Cafes.CreateCafe.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.API.Endpoints.Cafes;

public static class CreateCafeEndpoint
{
    public static RouteGroupBuilder MapCreateCafe(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            [FromBody] CreateCafeRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new CreateCafeCommand(request.Name, request.ContactInfo);
            var result = await mediator.Send<CreateCafeCommand, Result<CreateCafeResponse>>(command, ct);
            return result.ToCreatedResult(response => CafeRoutes.GetCafeLocation(response.CafeId));
        })
        .WithName("CreateCafe")
        .WithSummary("Create a new cafe")
        .Produces<CreateCafeResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        return group;
    }
}
