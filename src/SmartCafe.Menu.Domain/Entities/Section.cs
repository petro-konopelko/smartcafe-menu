using SmartCafe.Menu.Domain.Common;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Domain.Extensions;
using SmartCafe.Menu.Domain.Models;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Shared.Providers.Abstractions;

namespace SmartCafe.Menu.Domain.Entities;

public class Section : Entity
{
    private readonly List<MenuItem> _items = [];

    public Guid MenuId { get; }
    public Menu Menu { get; init; } = null!;
    public string Name { get; private set; } = string.Empty;
    public int Position { get; private set; }
    public TimeSpan? AvailableFrom { get; private set; }
    public TimeSpan? AvailableTo { get; private set; }
    public IReadOnlyCollection<MenuItem> Items => _items.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Is required for EF Core
    private Section() { }

    private Section(
        Guid sectionId,
        Guid menuId)
    {
        Id = sectionId;
        MenuId = menuId;
    }

    internal static Section Create(
        Guid sectionId,
        Guid menuId,
        DateTime createdAt)
    {
        if (sectionId.Equals(Guid.Empty))
        {
            throw new ArgumentException("ID cannot be empty", nameof(sectionId));
        }

        if (menuId.Equals(Guid.Empty))
        {
            throw new ArgumentException("Menu ID cannot be empty", nameof(menuId));
        }

        if (createdAt == default)
        {
            throw new ArgumentException("CreatedAt must be a valid date", nameof(createdAt));
        }

        return new(sectionId, menuId)
        {
            CreatedAt = createdAt
        };
    }

    internal Result UpdateDetails(
        string name,
        int position,
        TimeSpan? availableFrom,
        TimeSpan? availableTo,
        DateTime updatedAt)
    {
        var errors = new List<ErrorDetail>();

        if (position < 0)
        {
            throw new ArgumentException("Position cannot be negative", nameof(position));
        }

        if (updatedAt == default)
        {
            throw new ArgumentException("UpdatedAt must be a valid date", nameof(updatedAt));
        }

        var trimmedName = name?.Trim();

        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            errors.Add(new ErrorDetail("Section name is required", SectionErrorCodes.SectionNameRequired));
        }

        if (availableFrom.HasValue && availableTo.HasValue && availableFrom.Value > availableTo.Value)
        {
            errors.Add(new ErrorDetail("AvailableFrom must be earlier than or equal to AvailableTo", SectionErrorCodes.InvalidAvailabilityWindow));
        }

        if (errors.Count > 0)
        {
            return Result.Failure(Error.Validation(errors));
        }

        Name = trimmedName!;
        Position = position;
        AvailableFrom = availableFrom;
        AvailableTo = availableTo;
        UpdatedAt = updatedAt;

        return Result.Success();
    }

    internal Result SyncItems(
        IGuidIdProvider idProvider,
        IReadOnlyCollection<ItemUpdateInfo> items,
        DateTime utcNow)
    {
        ArgumentNullException.ThrowIfNull(idProvider);

        if (utcNow == default)
        {
            throw new ArgumentException("UpdatedAt must be a valid date", nameof(utcNow));
        }

        if (items.Count > 100)
        {
            return Result.Failure(Error.Validation(new ErrorDetail("Section cannot contain more than 100 items", SectionErrorCodes.TooManyItems)));
        }

        if (items.HasDuplicateByKey(i => i.Id, id => id.HasValue))
        {
            return Result.Failure(Error.Validation(new ErrorDetail("Item IDs must be unique", ItemErrorCodes.DuplicateItemId)));
        }

        if (items.HasDuplicateByKey(i => i.Name?.Trim().ToLowerInvariant(), name => !string.IsNullOrWhiteSpace(name)))
        {
            return Result.Failure(Error.Validation(new ErrorDetail("Item names must be unique within a section", ItemErrorCodes.ItemNameNotUnique)));
        }

        var result = _items.SyncCollection(
              items,
              itemId => new ErrorDetail($"Item with ID {itemId} not found", ItemErrorCodes.ItemNotFound),
              () => MenuItem.Create(idProvider.NewId(), Id, utcNow),
              (menuItem, itemInfo, position) => menuItem.UpdateDetails(itemInfo, position, utcNow));

        if (result.IsFailure)
        {
            return result;
        }

        UpdatedAt = utcNow;

        return Result.Success();
    }
}
