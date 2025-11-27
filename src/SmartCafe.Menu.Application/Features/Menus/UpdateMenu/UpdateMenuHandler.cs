using SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain;
using SmartCafe.Menu.Domain.Common;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.Application.Features.Menus.UpdateMenu;

public class UpdateMenuHandler(
    IMenuRepository menuRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<UpdateMenuRequest, Result<UpdateMenuResponse>>
{

    public async Task<Result<UpdateMenuResponse>> HandleAsync(
        UpdateMenuRequest request,
        CancellationToken cancellationToken = default)
    {
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

        // Build menu structure with preserved existing data
        var updatedMenu = new Domain.Entities.Menu
        {
            Id = menu.Id,
            CafeId = menu.CafeId,
            Name = request.Name,
            IsActive = menu.IsActive,
            IsPublished = menu.IsPublished,
            PublishedAt = menu.PublishedAt,
            ActivatedAt = menu.ActivatedAt,
            CreatedAt = menu.CreatedAt,
            UpdatedAt = now,
            Sections = []
        };

        // Build menu structure, passing original menu to preserve existing entity data
        BuildMenuStructure(updatedMenu, menu, request, now);

        // Save changes
        await menuRepository.UpdateAsync(updatedMenu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain event
        var domainEvent = new MenuUpdatedEvent(
            Guid.CreateVersion7(),
            updatedMenu.Id,
            updatedMenu.CafeId,
            updatedMenu.Name,
            now
        );
        await eventPublisher.PublishAsync(domainEvent, cancellationToken);

        return Result<UpdateMenuResponse>.Success(new UpdateMenuResponse(
            updatedMenu.Id,
            updatedMenu.CafeId,
            updatedMenu.Name,
            updatedMenu.IsActive,
            updatedMenu.IsPublished,
            updatedMenu.UpdatedAt
        ));
    }

    private async Task<Result<UpdateMenuResponse>?> ValidateCategoriesExist(List<Guid> categoryIds, CancellationToken cancellationToken)
    {
        if (!categoryIds.Any())
        {
            return null;
        }

        var foundCategories = await categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);
        var missingCategoryIds = categoryIds.Except(foundCategories.Select(c => c.Id)).ToList();

        if (missingCategoryIds.Any())
        {
            return Result<UpdateMenuResponse>.Failure(Error.Validation(
                new ErrorDetail($"Categories not found: {string.Join(", ", missingCategoryIds)}", ErrorCodes.CategoriesNotFound)));
        }

        return null;
    }

    private void BuildMenuStructure(
        Domain.Entities.Menu updatedMenu,
        Domain.Entities.Menu originalMenu,
        UpdateMenuRequest request,
        DateTime timestamp)
    {
        // Create lookup maps for existing sections and items to preserve their data
        var existingSections = originalMenu.Sections.ToDictionary(s => s.Id);
        var existingItems = originalMenu.Sections
            .SelectMany(s => s.Items)
            .ToDictionary(i => i.Id);

        foreach (var sectionDto in request.Sections)
        {
            // Use existing ID if provided, otherwise create new (for new sections)
            var sectionId = sectionDto.Id ?? Guid.CreateVersion7();
            var isExistingSection = sectionDto.Id.HasValue && existingSections.ContainsKey(sectionDto.Id.Value);

            var section = new Section
            {
                Id = sectionId,
                MenuId = updatedMenu.Id,
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

            updatedMenu.Sections.Add(section);
        }
    }
}
