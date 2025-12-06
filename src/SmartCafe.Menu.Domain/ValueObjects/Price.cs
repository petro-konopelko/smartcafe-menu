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

    public static Price Create(decimal amount, PriceUnit unit, decimal discount = 0)
    {
        if (amount <= 0)
            throw new ArgumentException("Price amount must be greater than zero", nameof(amount));

        if (discount < 0 || discount > 1)
            throw new ArgumentException("Discount must be between 0 and 1", nameof(discount));

        return new Price(amount, unit, discount);
    }
}
