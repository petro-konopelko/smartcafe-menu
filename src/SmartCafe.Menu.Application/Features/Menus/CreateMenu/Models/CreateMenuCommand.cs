using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.CreateMenu.Models;

public record CreateMenuCommand(
    Guid CafeId,
    string Name,
    List<SectionDto> Sections
) : ICommand<Result<CreateMenuResponse>>;
