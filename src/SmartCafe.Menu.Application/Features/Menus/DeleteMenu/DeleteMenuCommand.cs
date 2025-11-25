using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.DeleteMenu;

public record DeleteMenuCommand(
    Guid CafeId,
    Guid MenuId
) : ICommand<DeleteMenuResponse>;

public record DeleteMenuResponse(bool Success);
