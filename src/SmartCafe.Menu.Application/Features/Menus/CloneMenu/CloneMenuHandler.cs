using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Events;
using SmartCafe.Menu.Domain.Exceptions;
using SmartCafe.Menu.Domain.Interfaces;

namespace SmartCafe.Menu.Application.Features.Menus.CloneMenu;

public class CloneMenuHandler(
    IMenuRepository menuRepository,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<CloneMenuRequest, CloneMenuResponse>
{
    public async Task<CloneMenuResponse> HandleAsync(
        CloneMenuRequest request,
        CancellationToken cancellationToken = default)
    {
        // Load source menu
        var sourceMenu = await menuRepository.GetByIdAsync(request.SourceMenuId, cancellationToken);
        if (sourceMenu == null)
        {
            throw new MenuNotFoundException(request.SourceMenuId);
        }

        // Verify cafe ownership
        if (sourceMenu.CafeId != request.CafeId)
        {
            throw new MenuNotFoundException(request.SourceMenuId);
        }

        // Create new menu as draft copy
        var now = dateTimeProvider.UtcNow;
        var clonedMenu = new Domain.Entities.Menu
        {
            Id = Guid.CreateVersion7(),
            CafeId = request.CafeId,
            Name = request.NewMenuName,
            IsActive = false,
            IsPublished = false,
            CreatedAt = now,
            UpdatedAt = now,
            Sections = []
        };

        // Clone all sections and items
        CloneMenuStructure(clonedMenu, sourceMenu, now);

        // Save cloned menu
        await menuRepository.CreateAsync(clonedMenu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish domain event
        var domainEvent = new MenuClonedEvent(
            Guid.CreateVersion7(),
            clonedMenu.Id,
            request.SourceMenuId,
            request.CafeId,
            clonedMenu.Name,
            now
        );
        await eventPublisher.PublishAsync(domainEvent, cancellationToken);

        return new CloneMenuResponse(
            clonedMenu.Id,
            clonedMenu.CafeId,
            clonedMenu.Name,
            clonedMenu.IsActive,
            clonedMenu.IsPublished,
            clonedMenu.CreatedAt
        );
    }

    private void CloneMenuStructure(Domain.Entities.Menu clonedMenu, Domain.Entities.Menu sourceMenu, DateTime timestamp)
    {
        foreach (var sourceSection in sourceMenu.Sections)
        {
            var clonedSection = new Section
            {
                Id = Guid.CreateVersion7(),
                MenuId = clonedMenu.Id,
                Name = sourceSection.Name,
                DisplayOrder = sourceSection.DisplayOrder,
                AvailableFrom = sourceSection.AvailableFrom,
                AvailableTo = sourceSection.AvailableTo,
                CreatedAt = timestamp,
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
                    ImageBigUrl = sourceItem.ImageBigUrl,
                    ImageCroppedUrl = sourceItem.ImageCroppedUrl,
                    IsActive = sourceItem.IsActive,
                    IngredientOptions = sourceItem.IngredientOptions.ToList(),
                    CreatedAt = timestamp,
                    UpdatedAt = timestamp,
                    MenuItemCategories = []
                };

                // Clone category associations
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

            clonedMenu.Sections.Add(clonedSection);
        }
    }
}
