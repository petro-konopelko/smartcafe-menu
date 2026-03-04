using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Domain.ValueObjects;

/// <summary>
/// Represents a price with amount, unit, and optional discount.
/// </summary>
public sealed record Price
{
    public decimal Amount { get; init; }
    public PriceUnit Unit { get; init; }
    public decimal DiscountPercent { get; init; }

    /// <summary>
    /// Gets the final amount after applying discount.
    /// </summary>
    public decimal FinalAmount => Amount * (1 - (DiscountPercent / 100m));

    private Price(decimal amount, PriceUnit unit, decimal discountPercent)
    {
        Amount = amount;
        Unit = unit;
        DiscountPercent = discountPercent;
    }

    internal static Result<Price> Create(decimal amount, PriceUnit unit, decimal discountPercent)
    {
        List<ErrorDetail> errors = [];

        if (amount <= 0)
        {
            errors.Add(new ErrorDetail("Price amount must be greater than zero", ItemErrorCodes.PriceInvalid));
        }

        if (discountPercent < 0 || discountPercent >= 100)
        {
            errors.Add(new ErrorDetail("Discount must be between 0 and 100", ItemErrorCodes.PriceDiscountInvalid));
        }

        return errors.Count > 0
            ? Result<Price>.Failure(Error.Validation(errors))
            : Result<Price>.Success(new Price(amount, unit, discountPercent));
    }
}
