using SmartCafe.Menu.Application.Features.Cafes.Shared.Models;
using CafeEntity = SmartCafe.Menu.Domain.Entities.Cafe;

namespace SmartCafe.Menu.Application.Features.Cafes.Shared.Mappers;

public static class CafeMapper
{
    public static CafeDto ToCafeDto(this CafeEntity cafe)
    {
        ArgumentNullException.ThrowIfNull(cafe);

        return new CafeDto(
            cafe.Id,
            cafe.Name,
            cafe.ContactInfo,
            cafe.CreatedAt,
            cafe.UpdatedAt
        );
    }
}
