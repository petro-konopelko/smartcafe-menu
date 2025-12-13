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
    public decimal Discount { get; init; }

    /// <summary>
    /// Gets the final amount after applying discount.
    /// </summary>
    public decimal FinalAmount => Amount * (1 - Discount);

    private Price(decimal amount, PriceUnit unit, decimal discount)
    {
        Amount = amount;
        Unit = unit;
        Discount = discount;
    }

    public static Result<Price> Create(decimal amount, PriceUnit unit, decimal discount)
    {
        List<ErrorDetail> errors = [];

        if (amount <= 0)
        {
            errors.Add(new ErrorDetail("Price amount must be greater than zero", ItemErrorCodes.PriceInvalid));
        }

        if (discount < 0 || discount >= 1)
        {
            errors.Add(new ErrorDetail("Discount must be between 0 and 1", ItemErrorCodes.PriceDiscountInvalid));
        }

        return errors.Count > 0
            ? Result<Price>.Failure(Error.Validation(errors))
            : Result<Price>.Success(new Price(amount, unit, discount));
    }
}
