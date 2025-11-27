using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.Application.Features.Menus.PublishMenu.Models;

public record PublishMenuCommand(
    Guid CafeId,
    Guid MenuId
) : ICommand<Result<PublishMenuResponse>>;
