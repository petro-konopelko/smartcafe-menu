using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.Application.Features.Cafes.DeleteCafe.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.API.Endpoints.Cafes;

public static class DeleteCafeEndpoint
{
    public static RouteGroupBuilder MapDeleteCafe(this RouteGroupBuilder group)
    {
        group.MapDelete("/{cafeId:guid}", async (
            Guid cafeId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new DeleteCafeCommand(cafeId);
            var result = await mediator.Send<DeleteCafeCommand, Result>(command, ct);
            return result.ToNoContentResult();
        })
        .WithName("DeleteCafe")
        .WithSummary("Soft delete a cafe")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        return group;
    }
}
