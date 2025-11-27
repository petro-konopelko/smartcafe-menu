using SmartCafe.Menu.Application.Common.Results;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Features.Menus.CreateMenu.Models;

public record CreateMenuRequest(
    Guid CafeId,
    string Name,
    List<SectionDto> Sections
) : ICommand<Result<CreateMenuResponse>>;
