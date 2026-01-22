using SmartCafe.Menu.Application.Features.Cafes.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Cafes.ListCafes.Models;

public record ListCafesResponse(
    List<CafeDto> Cafes
);
