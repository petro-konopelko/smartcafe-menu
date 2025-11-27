using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.Application.Features.Menus.CreateMenu.Models;

public record CreateMenuRequest(
    Guid CafeId,
    string Name,
    List<SectionDto> Sections
) : ICommand<Result<CreateMenuResponse>>;
