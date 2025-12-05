using SmartCafe.Menu.Domain.Common;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.Domain.Entities;

public class Menu : Entity
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid CafeId { get; init; }
    public Cafe Cafe { get; init; } = null!;
    public string Name { get; private set; } = string.Empty;
    public MenuState State { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public DateTime? ActivatedAt { get; private set; }
    public List<Section> Sections { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; private set; }


    public Result Activate(IDateTimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(clock);

        if (State == MenuState.Deleted)
            return Result.Failure(Error.Validation(new ErrorDetail("Menu is deleted", ErrorCodes.MenuNotFound)));
        if (State != MenuState.Published)
            return Result.Failure(Error.Conflict("Menu is not published", ErrorCodes.MenuNotPublished));
        if (State == MenuState.Active)
            return Result.Failure(Error.Conflict("Menu is already active", ErrorCodes.MenuAlreadyActive));

        if (Sections.Count == 0)
            return Result.Failure(Error.Validation(new ErrorDetail("Menu must have at least one section", ErrorCodes.MenuHasNoSections)));

        var hasItems = Sections.Any(s => s.Items.Count > 0);
        if (!hasItems)
            return Result.Failure(Error.Validation(new ErrorDetail("Menu must have at least one item", ErrorCodes.MenuHasNoItems)));

        State = MenuState.Active;
        ActivatedAt = clock.UtcNow;
        UpdatedAt = clock.UtcNow;

        AddDomainEvent(new MenuActivatedEvent(
            clock.UtcNow,
            Id,
            CafeId,
            Name));

        return Result.Success();
    }

    public Result Publish(IDateTimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(clock);

        if (State == MenuState.Deleted)
            return Result.Failure(Error.Validation(new ErrorDetail("Menu is deleted", ErrorCodes.MenuNotFound)));
        if (State == MenuState.Published)
            return Result.Failure(Error.Conflict("Menu is already published", ErrorCodes.MenuAlreadyPublished));

        if (Sections.Count == 0)
            return Result.Failure(Error.Validation(new ErrorDetail("Menu must have at least one section", ErrorCodes.MenuHasNoSections)));

        var hasItems = Sections.Any(s => s.Items.Count > 0);
        if (!hasItems)
            return Result.Failure(Error.Validation(new ErrorDetail("Menu must have at least one item", ErrorCodes.MenuHasNoItems)));

        State = MenuState.Published;
        PublishedAt = clock.UtcNow;
        UpdatedAt = clock.UtcNow;

        AddDomainEvent(new MenuPublishedEvent(
            clock.UtcNow,
            Id,
            CafeId,
            Name));

        return Result.Success();
    }

    public Result Deactivate(IDateTimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(clock);

        if (State == MenuState.Deleted)
            return Result.Failure(Error.Validation(new ErrorDetail("Menu is deleted", ErrorCodes.MenuNotFound)));
        if (State != MenuState.Active)
            return Result.Failure(Error.Conflict("Menu is not active", ErrorCodes.MenuNotActive));

        State = MenuState.Published;
        UpdatedAt = clock.UtcNow;

        AddDomainEvent(new MenuDeactivatedEvent(
            clock.UtcNow,
            Id,
            CafeId));

        return Result.Success();
    }

    public Result SoftDelete(IDateTimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(clock);

        if (State == MenuState.Active)
            return Result.Failure(Error.Conflict("Cannot delete an active menu", ErrorCodes.CannotDeleteActiveMenu));

        if (State == MenuState.Deleted)
            return Result.Success();

        State = MenuState.Deleted;
        UpdatedAt = clock.UtcNow;

        AddDomainEvent(new MenuDeletedEvent(
            clock.UtcNow,
            Id,
            CafeId));

        return Result.Success();
    }

    public Result UpdateName(string name, IDateTimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(clock);

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation(new ErrorDetail("Menu name is required", ErrorCodes.MenuNameRequired)));

        Name = name.Trim();
        UpdatedAt = clock.UtcNow;
        return Result.Success();
    }

    public Result MarkUpdated(IDateTimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(clock);

        UpdatedAt = clock.UtcNow;
        AddDomainEvent(new MenuUpdatedEvent(
            clock.UtcNow,
            Id,
            CafeId,
            Name));
        return Result.Success();
    }

    public static Result<Menu> CreateDraft(Guid cafeId, string name, IDateTimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(clock);

        if (string.IsNullOrWhiteSpace(name))
            return Result<Menu>.Failure(Error.Validation(new ErrorDetail("Menu name is required", ErrorCodes.MenuNameRequired)));

        var menu = new Menu
        {
            Id = Guid.CreateVersion7(),
            CafeId = cafeId,
            CreatedAt = clock.UtcNow,
        };
        menu.Name = name.Trim();
        menu.State = MenuState.Draft;
        menu.UpdatedAt = menu.CreatedAt;

        menu.AddDomainEvent(new MenuCreatedEvent(
            clock.UtcNow,
            menu.Id,
            cafeId,
            menu.Name));

        return Result<Menu>.Success(menu);
    }

    public static Result<Menu> CloneFrom(Menu source, string newName, IDateTimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(clock);

        if (string.IsNullOrWhiteSpace(newName))
            return Result<Menu>.Failure(Error.Validation(new ErrorDetail("Menu name is required", ErrorCodes.MenuNameRequired)));

        var cloned = new Menu
        {
            Id = Guid.CreateVersion7(),
            CafeId = source.CafeId,
            CreatedAt = clock.UtcNow,
        };
        cloned.Name = newName.Trim();
        cloned.State = MenuState.Draft;
        cloned.UpdatedAt = cloned.CreatedAt;

        // Clone sections and items
        foreach (var sourceSection in source.Sections)
        {
            var clonedSection = new Section
            {
                Id = Guid.CreateVersion7(),
                MenuId = cloned.Id,
                Name = sourceSection.Name,
                DisplayOrder = sourceSection.DisplayOrder,
                AvailableFrom = sourceSection.AvailableFrom,
                AvailableTo = sourceSection.AvailableTo,
                CreatedAt = cloned.CreatedAt,
                Items = []
            };

            foreach (var sourceItem in sourceSection.Items)
            {
                var clonedItem = new MenuItem
                {
                    Id = Guid.CreateVersion7(),
                    SectionId = clonedSection.Id,
                    Name = sourceItem.Name,
                    Description = sourceItem.Description,
                    Price = sourceItem.Price,
                    Image = sourceItem.Image,
                    IsActive = sourceItem.IsActive,
                    IngredientOptions = sourceItem.IngredientOptions.ToList(),
                    CreatedAt = cloned.CreatedAt,
                    UpdatedAt = cloned.UpdatedAt,
                    MenuItemCategories = []
                };

                foreach (var sourceCategory in sourceItem.MenuItemCategories)
                {
                    clonedItem.MenuItemCategories.Add(new MenuItemCategory
                    {
                        MenuItemId = clonedItem.Id,
                        CategoryId = sourceCategory.CategoryId
                    });
                }

                clonedSection.Items.Add(clonedItem);
            }

            cloned.Sections.Add(clonedSection);
        }

        cloned.AddDomainEvent(new MenuClonedEvent(
            clock.UtcNow,
            source.Id,
            cloned.Id,
            source.CafeId,
            cloned.Name));

        return Result<Menu>.Success(cloned);
    }
}
