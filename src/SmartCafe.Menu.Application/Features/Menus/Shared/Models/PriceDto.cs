using SmartCafe.Menu.Domain.Enums;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Models;

public record PriceDto(
    decimal Amount,
    PriceUnit Unit,
    decimal Discount
);
