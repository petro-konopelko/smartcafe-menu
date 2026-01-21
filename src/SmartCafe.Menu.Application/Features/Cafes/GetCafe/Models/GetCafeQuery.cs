using SmartCafe.Menu.Application.Features.Cafes.Shared.Models;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Cafes.GetCafe.Models;

public record GetCafeQuery(
    Guid CafeId
) : IQuery<Result<CafeDto>>;
