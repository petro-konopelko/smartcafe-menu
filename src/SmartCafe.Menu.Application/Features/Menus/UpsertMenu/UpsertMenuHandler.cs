using SmartCafe.Menu.Application.Features.Menus.UpsertMenu;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Domain.Interfaces;
using SmartCafe.Menu.Domain.ValueObjects;

namespace SmartCafe.Menu.Application.Features.Menus.UpsertMenu;

public class UpsertMenuHandler(
    IMenuRepository menuRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    IDateTimeProvider dateTimeProvider)
{
    public async Task<UpsertMenuResponse> HandleAsync(
        Guid cafeId,
        UpsertMenuRequest request,
        CancellationToken cancellationToken = default)
    {
        var isUpdate = request.MenuId.HasValue;
        Domain.Entities.Menu menu;

        if (isUpdate)
        {
            menu = await UpdateMenuAsync(cafeId, request, cancellationToken);
        }
        else
        {
            menu = CreateMenu(cafeId, request);
        }

        // Validate categories exist
        await ValidateCategoriesAsync(request, cancellationToken);

        // Build sections and items
        BuildMenuStructure(menu, request);

        // Save to database
        if (isUpdate)
        {
            await menuRepository.UpdateAsync(menu, cancellationToken);
        }
        else
        {
            await menuRepository.CreateAsync(menu, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish event
        await PublishEventAsync(menu, cafeId, isUpdate, cancellationToken);

        return new UpsertMenuResponse(
            menu.Id,
            menu.Name,
            request.Description,
            menu.IsPublished ? "Published" : "Draft",
            menu.CreatedAt,
            menu.UpdatedAt
        );
    }

    private async Task<Domain.Entities.Menu> UpdateMenuAsync(
        Guid cafeId,
        UpsertMenuRequest request,
        CancellationToken cancellationToken)
    {
        var existingMenu = await menuRepository.GetByIdAsync(request.MenuId!.Value, cancellationToken);
        if (existingMenu == null || existingMenu.CafeId != cafeId)
        {
            throw new InvalidOperationException("Menu not found");
        }

        // Create new menu instance with updated data (Name is init-only)
        var updatedMenu = new Domain.Entities.Menu
        {
            Id = existingMenu.Id,
            CafeId = existingMenu.CafeId,
            Name = request.Name,
            IsActive = existingMenu.IsActive,
            IsPublished = existingMenu.IsPublished,
            PublishedAt = existingMenu.PublishedAt,
            ActivatedAt = existingMenu.ActivatedAt,
            Sections = []
        };

        updatedMenu.GetType().GetProperty(nameof(Domain.Entities.Menu.CreatedAt))!
            .SetValue(updatedMenu, existingMenu.CreatedAt);
        updatedMenu.GetType().GetProperty(nameof(Domain.Entities.Menu.UpdatedAt))!
            .SetValue(updatedMenu, dateTimeProvider.UtcNow);

        return updatedMenu;
    }

    private Domain.Entities.Menu CreateMenu(Guid cafeId, UpsertMenuRequest request)
    {
        var menu = new Domain.Entities.Menu
        {
            Id = Guid.CreateVersion7(),
            CafeId = cafeId,
            Name = request.Name,
            IsActive = false,
            IsPublished = false,
            Sections = []
        };

        menu.GetType().GetProperty(nameof(Domain.Entities.Menu.CreatedAt))!
            .SetValue(menu, dateTimeProvider.UtcNow);
        menu.GetType().GetProperty(nameof(Domain.Entities.Menu.UpdatedAt))!
            .SetValue(menu, dateTimeProvider.UtcNow);

        return menu;
    }

    private async Task ValidateCategoriesAsync(UpsertMenuRequest request, CancellationToken cancellationToken)
    {
        var allCategoryIds = request.Sections
            .SelectMany(s => s.Items)
            .SelectMany(i => i.CategoryIds)
            .Distinct()
            .ToList();

        foreach (var categoryId in allCategoryIds)
        {
            var category = await categoryRepository.GetByIdAsync(categoryId, cancellationToken);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {categoryId} not found");
            }
        }
    }

    private void BuildMenuStructure(Domain.Entities.Menu menu, UpsertMenuRequest request)
    {
        foreach (var sectionDto in request.Sections)
        {
            var section = new Section
            {
                Id = sectionDto.SectionId ?? Guid.CreateVersion7(),
                MenuId = menu.Id,
                Name = sectionDto.Name,
                AvailableFrom = sectionDto.AvailableFrom,
                AvailableTo = sectionDto.AvailableTo,
                DisplayOrder = sectionDto.DisplayOrder,
                Items = []
            };

            foreach (var itemDto in sectionDto.Items)
            {
                var item = new MenuItem
                {
                    Id = itemDto.ItemId ?? Guid.CreateVersion7(),
                    SectionId = section.Id,
                    Name = itemDto.Name,
                    Description = itemDto.Description ?? string.Empty,
                    Price = itemDto.Price,
                    ImageBigUrl = itemDto.BigImageUrl,
                    ImageCroppedUrl = itemDto.CroppedImageUrl,
                    IsActive = itemDto.IsAvailable,
                    IngredientOptions = itemDto.Ingredients
                        .Select(i => new Ingredient
                        {
                            Name = i.Name,
                            IsExcludable = i.CanBeExcluded,
                            IsIncludable = i.CanBeIncluded
                        }).ToList(),
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

    private async Task PublishEventAsync(
        Domain.Entities.Menu menu,
        Guid cafeId,
        bool isUpdate,
        CancellationToken cancellationToken)
    {
        object eventToPublish = isUpdate
            ? new MenuUpdatedEvent(
                Guid.CreateVersion7(),
                menu.Id,
                cafeId,
                menu.Name,
                dateTimeProvider.UtcNow)
            : (object)new MenuCreatedEvent(
                Guid.CreateVersion7(),
                menu.Id,
                cafeId,
                menu.Name,
                dateTimeProvider.UtcNow);

        await eventPublisher.PublishAsync(eventToPublish, cancellationToken);
    }
}
