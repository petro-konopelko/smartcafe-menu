using SmartCafe.Menu.API.Extensions;
using SmartCafe.Menu.Application.Common.Results;
using SmartCafe.Menu.Application.Features.Menus.PublishMenu.Models;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.API.Endpoints.Menus;

public static class PublishMenuEndpoint
{
    public static RouteGroupBuilder MapPublishMenu(this RouteGroupBuilder group)
    {
        group.MapPost("/{menuId:guid}/publish", async (
            Guid cafeId,
            Guid menuId,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = new PublishMenuCommand(cafeId, menuId);
            var result = await mediator.Send<PublishMenuCommand, Result<PublishMenuResponse>>(command, ct);
            return result.ToApiResult();
        })
        .WithName("PublishMenu")
        .WithSummary("Publish a menu (makes it available for activation)")
        .Produces<PublishMenuResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        return group;
    }
}
