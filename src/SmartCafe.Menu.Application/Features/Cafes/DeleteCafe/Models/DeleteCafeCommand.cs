using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Cafes.DeleteCafe.Models;

public record DeleteCafeCommand(
    Guid CafeId
) : ICommand<Result>;
