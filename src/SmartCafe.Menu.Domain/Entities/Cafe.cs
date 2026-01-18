using SmartCafe.Menu.Domain.Common;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Shared.Providers.Abstractions;

namespace SmartCafe.Menu.Domain.Entities;

public class Cafe : Entity
{
    public required string Name { get; init; }
    public string? ContactInfo { get; init; }
    public DateTime CreatedAt { get; init; }

    // Private constructor for EF Core materialization
    private Cafe() { }

    public static Result<Cafe> Create(Guid cafeId, string name, IDateTimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(clock);

        return Create(cafeId, name, clock.UtcNow);
    }

    public static Result<Cafe> Create(Guid cafeId, string name, DateTime utcNow)
    {
        if (cafeId.Equals(Guid.Empty))
        {
            throw new ArgumentException("ID cannot be empty", nameof(cafeId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Cafe>.Failure(Error.Validation(new ErrorDetail("Cafe name is required", CafeErrorCodes.CafeNameRequired)));
        }

        var cafe = new Cafe
        {
            Id = cafeId,
            Name = name,
            CreatedAt = utcNow
        };

        return Result<Cafe>.Success(cafe);
    }
}
