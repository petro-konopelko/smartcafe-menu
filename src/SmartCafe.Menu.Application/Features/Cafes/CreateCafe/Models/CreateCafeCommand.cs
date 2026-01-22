using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Cafes.CreateCafe.Models;

public record CreateCafeCommand(
    string Name,
    string? ContactInfo
) : ICommand<Result<CreateCafeResponse>>;
