using SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Mappers;
using SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain;
using SmartCafe.Menu.Domain.Common;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.Application.Features.Menus.UpdateMenu;

public class UpdateMenuHandler(
    IMenuRepository menuRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<UpdateMenuRequest, Result<UpdateMenuResponse>>
{

    public async Task<Result<UpdateMenuResponse>> HandleAsync(
        UpdateMenuRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Load existing menu
        var menu = await menuRepository.GetByIdAsync(request.MenuId, cancellationToken);
        if (menu == null || menu.CafeId != request.CafeId)
        {
            return Result<UpdateMenuResponse>.Failure(Error.NotFound(
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
        if (categoryValidation != null)
        {
            return categoryValidation;
        }

        // Capture timestamp once for consistency across all updates
        var now = dateTimeProvider.UtcNow;

        // Update name via domain
        var nameUpdate = menu.UpdateName(request.Name, dateTimeProvider);
        if (nameUpdate.IsFailure)
            return Result<UpdateMenuResponse>.Failure(nameUpdate.Error!);

        // Build menu structure in-place, preserving existing entity data
        BuildMenuStructureInPlace(menu, request, now);

        // Mark updated to emit event
        var markUpdated = menu.MarkUpdated(dateTimeProvider);
        if (markUpdated.IsFailure)
            return Result<UpdateMenuResponse>.Failure(markUpdated.Error!);

        // Persist changes
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Dispatch domain events directly from menu
        var events = menu.DomainEvents.ToList();
        menu.ClearDomainEvents();
        await eventDispatcher.DispatchAsync(events, cancellationToken);

        return Result<UpdateMenuResponse>.Success(menu.ToUpdateMenuResponse());
    }

    private async Task<Result<UpdateMenuResponse>?> ValidateCategoriesExist(List<Guid> categoryIds, CancellationToken cancellationToken)
    {
        if (categoryIds.Count == 0)
        {
            return null;
        }

        var foundCategories = await categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);
        var missingCategoryIds = categoryIds.Except(foundCategories.Select(c => c.Id)).ToList();

        if (missingCategoryIds.Count != 0)
        {
            return Result<UpdateMenuResponse>.Failure(Error.Validation(
                new ErrorDetail($"Categories not found: {string.Join(", ", missingCategoryIds)}", ErrorCodes.CategoriesNotFound)));
        }

        return null;
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
            // Use existing ID if provided, otherwise create new (for new sections)
            var sectionId = sectionDto.Id ?? Guid.CreateVersion7();
            var isExistingSection = sectionDto.Id.HasValue && existingSections.ContainsKey(sectionDto.Id.Value);

            var section = new Section
            {
                Id = sectionId,
                MenuId = menu.Id,
                Name = sectionDto.Name,
                DisplayOrder = sectionDto.DisplayOrder,
                AvailableFrom = sectionDto.AvailableFrom,
                AvailableTo = sectionDto.AvailableTo,
                CreatedAt = isExistingSection ? existingSections[sectionId].CreatedAt : timestamp,
                Items = []
            };

            foreach (var itemDto in sectionDto.Items)
            {
                // Use existing ID if provided, otherwise create new (for new items)
                var itemId = itemDto.Id ?? Guid.CreateVersion7();
                var isExistingItem = itemDto.Id.HasValue && existingItems.ContainsKey(itemDto.Id.Value);
                var existingItem = isExistingItem ? existingItems[itemId] : null;

                var item = new MenuItem
                {
                    Id = itemId,
                    SectionId = section.Id,
                    Name = itemDto.Name,
                    Description = itemDto.Description,
                    Price = itemDto.Price,
                    ImageBigUrl = existingItem?.ImageBigUrl, // Preserve existing image URLs
                    ImageCroppedUrl = existingItem?.ImageCroppedUrl, // Preserve existing image URLs
                    IsActive = existingItem?.IsActive ?? true,
                    IngredientOptions = itemDto.IngredientOptions,
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
