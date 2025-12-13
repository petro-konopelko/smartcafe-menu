using SmartCafe.Menu.Domain.Common;
using SmartCafe.Menu.Domain.Enums;
using SmartCafe.Menu.Domain.Errors;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Domain.Extensions;
using SmartCafe.Menu.Domain.Models;
using SmartCafe.Menu.Shared.Models;
using SmartCafe.Menu.Shared.Providers.Abstractions;

namespace SmartCafe.Menu.Domain.Entities;

public class Menu : Entity
{
    private readonly List<Section> _sections = [];

    public Guid CafeId { get; init; }
    public Cafe Cafe { get; init; } = null!;
    public string Name { get; private set; } = string.Empty;
    public MenuState State { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public DateTime? ActivatedAt { get; private set; }
    public IReadOnlyCollection<Section> Sections => _sections.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Is required for EF Core
    private Menu() { }

    private Menu(Guid menuId, Guid cafeId)
    {
        if (menuId.Equals(Guid.Empty))
        {
            throw new ArgumentException("ID cannot be empty", nameof(menuId));
        }

        if (cafeId.Equals(Guid.Empty))
        {
            throw new ArgumentException("Cafe ID cannot be empty", nameof(cafeId));
        }

        Id = menuId;
        CafeId = cafeId;
    }

    public static Result<Menu> Create(
        Guid cafeId,
        string name,
        IGuidIdProvider idProvider,
        IDateTimeProvider clock,
        IReadOnlyCollection<SectionUpdateInfo> sections)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(idProvider);

        var menu = new Menu(idProvider.NewId(), cafeId);

        var utcNow = clock.UtcNow;

        menu.State = MenuState.New;
        menu.CreatedAt = utcNow;

        var result = menu.SyncMenu(name, sections, idProvider, utcNow);

        if (result.IsFailure)
        {
            return Result<Menu>.Failure(result.EnsureError());
        }

        menu.AddDomainEvent(new MenuCreatedEvent(
            clock.UtcNow,
            menu.Id,
            cafeId,
            menu.Name));

        return Result<Menu>.Success(menu);
    }

    public Result SyncMenu(
        string name,
        IReadOnlyCollection<SectionUpdateInfo> sections,
        IDateTimeProvider clock,
        IGuidIdProvider idProvider)
    {
        ArgumentNullException.ThrowIfNull(clock);

        var utcNow = clock.UtcNow;

        return SyncMenu(name, sections, idProvider, utcNow);
    }

    public Result Activate(IDateTimeProvider clock)
    {
        ArgumentNullException.ThrowIfNull(clock);

        if (State == MenuState.Deleted)
        {
            return Result.Failure(Error.Validation(new ErrorDetail("Menu is deleted", MenuErrorCodes.MenuNotFound)));
        }

        if (State != MenuState.Published)
        {
            return Result.Failure(Error.Conflict("Menu is not published", MenuErrorCodes.MenuNotPublished));
        }

        if (State == MenuState.Active)
        {
            return Result.Failure(Error.Conflict("Menu is already active", MenuErrorCodes.MenuAlreadyActive));
        }

        if (Sections.Count == 0)
        {
            return Result.Failure(Error.Validation(new ErrorDetail("Menu must have at least one section", MenuErrorCodes.MenuHasNoSections)));
        }

        var hasItems = Sections.Any(s => s.Items.Count > 0);
        if (!hasItems)
        {
            return Result.Failure(Error.Validation(new ErrorDetail("Menu must have at least one item", MenuErrorCodes.MenuHasNoItems)));
        }

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
        {
            return Result.Failure(Error.Validation(new ErrorDetail("Menu is deleted", MenuErrorCodes.MenuNotFound)));
        }

        if (State == MenuState.Published)
        {
            return Result.Failure(Error.Conflict("Menu is already published", MenuErrorCodes.MenuAlreadyPublished));
        }

        if (Sections.Count == 0)
        {
            return Result.Failure(Error.Validation(new ErrorDetail("Menu must have at least one section", MenuErrorCodes.MenuHasNoSections)));
        }

        var hasItems = Sections.Any(s => s.Items.Count > 0);
        if (!hasItems)
        {
            return Result.Failure(Error.Validation(new ErrorDetail("Menu must have at least one item", MenuErrorCodes.MenuHasNoItems)));
        }

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
        {
            return Result.Failure(Error.Validation(new ErrorDetail("Menu is deleted", MenuErrorCodes.MenuNotFound)));
        }

        if (State != MenuState.Active)
        {
            return Result.Failure(Error.Conflict("Menu is not active", MenuErrorCodes.MenuNotActive));
        }

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
        {
            return Result.Failure(Error.Conflict("Cannot delete an active menu", MenuErrorCodes.CannotDeleteActiveMenu));
        }

        if (State == MenuState.Deleted)
        {
            return Result.Success();
        }

        State = MenuState.Deleted;
        UpdatedAt = clock.UtcNow;

        AddDomainEvent(new MenuDeletedEvent(
            clock.UtcNow,
            Id,
            CafeId));

        return Result.Success();
    }

    private Result<string> ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<string>.Failure(Error.Validation(new ErrorDetail("Menu name is required", MenuErrorCodes.MenuNameRequired)));
        }

        return Result<string>.Success(name.Trim());
    }

    private Result SyncMenu(
        string name,
        IReadOnlyCollection<SectionUpdateInfo> sections,
        IGuidIdProvider idProvider,
        DateTime utcNow)
    {
        var nameResult = ValidateName(name);

        if (nameResult.IsFailure)
        {
            return nameResult;
        }

        var syncSectionsResult = SyncSections(sections, idProvider, utcNow);

        if (syncSectionsResult.IsFailure)
        {
            return syncSectionsResult;
        }

        UpdatedAt = utcNow;
        Name = nameResult.EnsureValue();

        return Result.Success();
    }

    private Result SyncSections(IReadOnlyCollection<SectionUpdateInfo> sections, IGuidIdProvider idProvider, DateTime utcNow)
    {
        if (sections.HasDuplicateByKey(s => s.Id, s => s.HasValue))
        {
            return Result.Failure(Error.Validation(new ErrorDetail("Section IDs must be unique", SectionErrorCodes.DuplicateSectionId)));
        }

        if (sections.HasDuplicateByKey(s => s.Name?.Trim().ToLowerInvariant(), name => !string.IsNullOrWhiteSpace(name)))
        {
            return Result.Failure(Error.Validation(new ErrorDetail("Section names must be unique", SectionErrorCodes.SectionNameNotUnique)));
        }

        return _sections.SyncCollection(
              sections,
              sectionId => new ErrorDetail($"Section with ID {sectionId} not found", SectionErrorCodes.SectionNotFound),
              () => Section.Create(idProvider.NewId(), Id, utcNow),
              (section, sectionInfo, position) => section.UpdateDetails(sectionInfo.Name, position, sectionInfo.AvailableFrom, sectionInfo.AvailableTo, utcNow),
              (section, sectionInfo) => section.SyncItems(idProvider, sectionInfo.Items, utcNow));
    }


}

