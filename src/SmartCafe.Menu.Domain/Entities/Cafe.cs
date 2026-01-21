using SmartCafe.Menu.Domain.Common;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Shared.Providers.Abstractions;

namespace SmartCafe.Menu.Domain.Entities;

public class Cafe : Entity
{
    public const int MaxNameLength = 200;
    public const int MaxContactInfoLength = 500;

    public required string Name { get; init; }
    public string? ContactInfo { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    // Private constructor for EF Core materialization
    private Cafe() { }

    public static Result<Cafe> Create(Guid cafeId, string name, IDateTimeProvider clock, string? contactInfo = null)
    {
        ArgumentNullException.ThrowIfNull(clock);

        if (cafeId.Equals(Guid.Empty))
        {
            throw new ArgumentException("ID cannot be empty", nameof(cafeId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<Cafe>.Failure(Error.Validation(new ErrorDetail("Cafe name is required", CafeErrorCodes.CafeNameRequired)));
        }

        if (name.Length > MaxNameLength)
        {
            return Result<Cafe>.Failure(Error.Validation(new ErrorDetail($"Cafe name cannot exceed {MaxNameLength} characters", CafeErrorCodes.CafeNameTooLong)));
        }

        if (contactInfo?.Length > MaxContactInfoLength)
        {
            return Result<Cafe>.Failure(Error.Validation(new ErrorDetail($"Contact info cannot exceed {MaxContactInfoLength} characters", CafeErrorCodes.CafeContactInfoTooLong)));
        }

        var cafe = new Cafe
        {
            Id = cafeId,
            Name = name,
            ContactInfo = contactInfo,
            CreatedAt = clock.UtcNow,
            IsDeleted = false
        };

        cafe.AddDomainEvent(new CafeCreatedEvent(cafe.CreatedAt, cafeId, name));

        return Result<Cafe>.Success(cafe);
    }

    public Result SoftDelete(IDateTimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(clock);

        if (IsDeleted)
        {
            return Result.Failure(Error.Conflict(
                "Cafe is already deleted",
                CafeErrorCodes.CafeAlreadyDeleted));
        }

        IsDeleted = true;
        UpdatedAt = clock.UtcNow;

        AddDomainEvent(new CafeDeletedEvent(clock.UtcNow, Id));

        return Result.Success();
    }
}
