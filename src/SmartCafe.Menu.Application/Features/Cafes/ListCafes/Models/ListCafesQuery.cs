using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Cafes.ListCafes.Models;

public record ListCafesQuery : IQuery<Result<ListCafesResponse>>;
