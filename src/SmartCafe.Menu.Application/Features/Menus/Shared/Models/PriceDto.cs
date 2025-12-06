using SmartCafe.Menu.Domain.ValueObjects;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Models;

public record PriceDto(
    decimal Amount,
    PriceUnit Unit,
    decimal Discount
);
