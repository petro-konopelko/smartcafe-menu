namespace SmartCafe.Menu.Application.Features.Cafes.Shared.Models;

public record CafeDto(
    Guid Id,
    string Name,
    string? ContactInfo,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
