using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Models;

public record GetActiveMenuQuery(
    Guid CafeId
) : IQuery<Result<MenuDto>>;
