using SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain;
using SmartCafe.Menu.Domain.Common;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Interfaces;
using SmartCafe.Menu.Domain.ValueObjects;

namespace SmartCafe.Menu.Application.Features.Menus.UpdateMenu;

public class UpdateMenuHandler(
    IMenuRepository menuRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<UpdateMenuRequest, Result>
{

    public async Task<Result> HandleAsync(
        UpdateMenuRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Load existing menu
        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);
        if (menu is null || menu.CafeId != request.CafeId)
        {
            return Result.Failure(Error.NotFound(
                $"Menu with ID {request.MenuId} not found",
                ErrorCodes.MenuNotFound));
        }

        // Validate all category IDs exist
        var allCategoryIds = request.Sections
            .SelectMany(s => s.Items)
            .SelectMany(i => i.CategoryIds)
            .Distinct()
            .ToList();

        var categoryValidation = await ValidateCategoriesExist(allCategoryIds, cancellationToken);
        if (categoryValidation is not null)
        {
            return categoryValidation;
        }

        // Capture timestamp once for consistency across all updates
        var now = dateTimeProvider.UtcNow;

        // Update name via domain
        var nameUpdate = menu.UpdateName(request.Name, dateTimeProvider);
        if (nameUpdate.IsFailure)
            return Result.Failure(nameUpdate.EnsureError());

        // Build menu structure in-place, preserving existing entity data
        BuildMenuStructureInPlace(menu, request, now);

        // Mark updated to emit event
        var markUpdated = menu.MarkUpdated(dateTimeProvider);
        if (markUpdated.IsFailure)
            return Result.Failure(markUpdated.EnsureError());

        // Persist changes
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Dispatch domain events directly from menu
        var events = menu.DomainEvents.ToList();
        menu.ClearDomainEvents();
        await eventDispatcher.DispatchAsync(events, cancellationToken);

        return Result.Success();
    }

    private async Task<Result?> ValidateCategoriesExist(List<Guid> categoryIds, CancellationToken cancellationToken)
    {
        if (categoryIds.Count == 0)
        {
            return null;
        }

        var foundCategories = await categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);
        var missingCategoryIds = categoryIds.Except(foundCategories.Select(c => c.Id)).ToList();

        return missingCategoryIds.Count != 0
            ? Result.Failure(Error.Validation(
                new ErrorDetail($"Categories not found: {string.Join(", ", missingCategoryIds)}", ErrorCodes.CategoriesNotFound)))
            : null;
    }

    private void BuildMenuStructureInPlace(
        Domain.Entities.Menu menu,
        UpdateMenuRequest request,
        DateTime timestamp)
    {
        // Create lookup maps for existing sections and items to preserve their data
        var existingSections = menu.Sections.ToDictionary(s => s.Id);
        var existingItems = menu.Sections
            .SelectMany(s => s.Items)
            .ToDictionary(i => i.Id);

        // Clear and rebuild sections
        menu.Sections.Clear();

        foreach (var sectionDto in request.Sections)
        {
            // Check if this is an existing section (has matching ID in dictionary)
            var isExistingSection = existingSections.ContainsKey(sectionDto.Id);

            var section = new Section
            {
                Id = sectionDto.Id,
                MenuId = menu.Id,
                Name = sectionDto.Name,
                DisplayOrder = sectionDto.DisplayOrder,
                AvailableFrom = sectionDto.AvailableFrom,
                AvailableTo = sectionDto.AvailableTo,
                CreatedAt = isExistingSection ? existingSections[sectionDto.Id].CreatedAt : timestamp,
                Items = []
            };

            foreach (var itemDto in sectionDto.Items)
            {
                // Check if this is an existing item (has matching ID in dictionary)
                var isExistingItem = existingItems.ContainsKey(itemDto.Id);
                var existingItem = isExistingItem ? existingItems[itemDto.Id] : null;

                var item = new MenuItem
                {
                    Id = itemDto.Id,
                    SectionId = section.Id,
                    Name = itemDto.Name,
                    Description = itemDto.Description,
                    Price = Price.Create(itemDto.Price.Amount, itemDto.Price.Unit, itemDto.Price.Discount),
                    Image = existingItem?.Image, // Preserve existing image
                    IsActive = itemDto.IsActive,
                    IngredientOptions = itemDto.Ingredients.Select(i => new Ingredient
                    {
                        Name = i.Name,
                        IsExcludable = i.IsExcludable,
                        IsIncludable = i.IsIncludable
                    }).ToList(),
                    CreatedAt = isExistingItem ? existingItem!.CreatedAt : timestamp,
                    UpdatedAt = timestamp,
                    MenuItemCategories = []
                };

                // Add category associations
                foreach (var categoryId in itemDto.CategoryIds)
                {
                    item.MenuItemCategories.Add(new MenuItemCategory
                    {
                        MenuItemId = item.Id,
                        CategoryId = categoryId
                    });
                }

                section.Items.Add(item);
            }

            menu.Sections.Add(section);
        }
    }
}
