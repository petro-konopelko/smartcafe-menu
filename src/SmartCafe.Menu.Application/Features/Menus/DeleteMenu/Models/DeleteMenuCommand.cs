using SmartCafe.Menu.Application.Common.Results;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.DeleteMenu.Models;

public record DeleteMenuCommand(
    Guid CafeId,
    Guid MenuId
) : ICommand<Result>;
