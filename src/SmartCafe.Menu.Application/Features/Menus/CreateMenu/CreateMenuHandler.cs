using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Features.Menus.Shared;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Domain.Interfaces;
using SmartCafe.Menu.Domain.ValueObjects;

namespace SmartCafe.Menu.Application.Features.Menus.CreateMenu;

public class CreateMenuHandler(
    IMenuRepository menuRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    IDateTimeProvider dateTimeProvider)
{
    public async Task<CreateMenuResponse> HandleAsync(
        Guid cafeId,
        CreateMenuRequest request,
        CancellationToken cancellationToken = default)
    {
        // Validate categories exist
        await ValidateCategoriesAsync(request, cancellationToken);

        // Create menu entity
        var now = dateTimeProvider.UtcNow;
        var menu = new Domain.Entities.Menu
        {
            Id = Guid.CreateVersion7(),
            CafeId = cafeId,
            Name = request.Name,
            IsActive = false,
            IsPublished = false,
            CreatedAt = now,
            UpdatedAt = now,
            Sections = []
        };

        // Build sections and items
        BuildMenuStructure(menu, request.Sections, now);

        // Save to database
        await menuRepository.CreateAsync(menu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish event
        await eventPublisher.PublishAsync(
            new MenuCreatedEvent(
                Guid.CreateVersion7(),
                menu.Id,
                cafeId,
                menu.Name,
                now),
            cancellationToken);

        return new CreateMenuResponse(
            menu.Id,
            menu.CafeId,
            menu.Name,
            menu.IsActive,
            menu.IsPublished,
            menu.CreatedAt
        );
    }

    private async Task ValidateCategoriesAsync(CreateMenuRequest request, CancellationToken cancellationToken)
    {
        var allCategoryIds = request.Sections
            .SelectMany(s => s.Items)
            .SelectMany(i => i.CategoryIds)
            .Distinct()
            .ToList();

        if (!allCategoryIds.Any())
        {
            return;
        }

        var foundCategories = await categoryRepository.GetByIdsAsync(allCategoryIds, cancellationToken);
        var missingCategoryIds = allCategoryIds.Except(foundCategories.Select(c => c.Id)).ToList();
        
        if (missingCategoryIds.Any())
        {
            throw new InvalidOperationException($"Categories not found: {string.Join(", ", missingCategoryIds)}");
        }
    }

    private void BuildMenuStructure(Domain.Entities.Menu menu, List<SectionDto> sections, DateTime timestamp)
    {
        foreach (var sectionDto in sections)
        {
            var section = new Section
            {
                Id = Guid.CreateVersion7(),
                MenuId = menu.Id,
                Name = sectionDto.Name,
                AvailableFrom = sectionDto.AvailableFrom,
                AvailableTo = sectionDto.AvailableTo,
                DisplayOrder = sectionDto.DisplayOrder,
                CreatedAt = timestamp,
                Items = []
            };

            foreach (var itemDto in sectionDto.Items)
            {
                var item = new MenuItem
                {
                    Id = Guid.CreateVersion7(),
                    SectionId = section.Id,
                    Name = itemDto.Name,
                    Description = itemDto.Description,
                    Price = itemDto.Price,
                    IsActive = true,
                    IngredientOptions = itemDto.IngredientOptions,
                    CreatedAt = timestamp,
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
