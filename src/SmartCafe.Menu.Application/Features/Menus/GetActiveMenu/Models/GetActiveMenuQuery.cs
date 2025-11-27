using SmartCafe.Menu.Application.Common.Results;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Models;

public record GetActiveMenuQuery(
    Guid CafeId
) : IQuery<Result<GetActiveMenuResponse>>;
