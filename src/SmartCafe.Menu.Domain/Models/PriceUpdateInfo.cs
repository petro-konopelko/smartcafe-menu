using SmartCafe.Menu.Domain.Enums;

namespace SmartCafe.Menu.Domain.Models;

/// <summary>
/// Update info for a price with individual fields for validation in domain.
/// </summary>
public record PriceUpdateInfo(
    decimal Amount,
    PriceUnit Unit,
    decimal Discount
);
