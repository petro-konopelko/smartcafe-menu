using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.Application.Features.Menus.DeleteMenu.Models;

public record DeleteMenuCommand(
    Guid CafeId,
    Guid MenuId
) : ICommand<Result>;
