using SmartCafe.Menu.Application.Features.Menus.CreateMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Mappers;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;
using SmartCafe.Menu.Application.Interfaces;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain;
using SmartCafe.Menu.Domain.Common;
using SmartCafe.Menu.Domain.Entities;
using SmartCafe.Menu.Domain.Interfaces;
using SmartCafe.Menu.Domain.ValueObjects;

namespace SmartCafe.Menu.Application.Features.Menus.CreateMenu;

public class CreateMenuHandler(
    ICafeRepository cafeRepository,
    IMenuRepository menuRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher eventDispatcher,
    IDateTimeProvider dateTimeProvider) : ICommandHandler<CreateMenuRequest, Result<CreateMenuResponse>>
{
    public async Task<Result<CreateMenuResponse>> HandleAsync(
        CreateMenuRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Check cafe exists
        var cafeExists = await cafeRepository.ExistsAsync(request.CafeId, cancellationToken);
        if (!cafeExists)
        {
            return Result<CreateMenuResponse>.Failure(Error.NotFound(
                $"Cafe with ID {request.CafeId} not found",
                ErrorCodes.CafeNotFound));
        }

        // Validate categories exist
        var categoryValidation = await ValidateCategoriesAsync(request, cancellationToken);
        if (categoryValidation is not null)
        {
            return categoryValidation;
        }

        var menuResult = Domain.Entities.Menu.CreateDraft(request.CafeId, request.Name, dateTimeProvider);

        if (menuResult.IsFailure)
            return Result<CreateMenuResponse>.Failure(menuResult.EnsureError());

        var menu = menuResult.EnsureValue();

        // Build sections and items
        var now = dateTimeProvider.UtcNow;
        BuildMenuStructure(menu, request.Sections, now);
        // Save to database
        await menuRepository.CreateAsync(menu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Dispatch domain events
        var events = menu.DomainEvents.ToList();
        menu.ClearDomainEvents();
        await eventDispatcher.DispatchAsync(events, cancellationToken);

        return Result<CreateMenuResponse>.Success(menu.ToCreateMenuResponse());
    }

    private async Task<Result<CreateMenuResponse>?> ValidateCategoriesAsync(CreateMenuRequest request, CancellationToken cancellationToken)
    {
        var allCategoryIds = request.Sections
            .SelectMany(s => s.Items)
            .SelectMany(i => i.CategoryIds)
            .Distinct()
            .ToList();

        if (allCategoryIds.Count == 0)
        {
            return null;
        }

        var foundCategories = await categoryRepository.GetByIdsAsync(allCategoryIds, cancellationToken);
        var missingCategoryIds = allCategoryIds.Except(foundCategories.Select(c => c.Id)).ToList();

        return missingCategoryIds.Count != 0
            ? Result<CreateMenuResponse>.Failure(Error.Validation(
                new ErrorDetail($"Categories not found: {string.Join(", ", missingCategoryIds)}", ErrorCodes.CategoriesNotFound)))
            : null;
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
                    IngredientOptions = itemDto.Ingredients.Select(i => new Ingredient
                    {
                        Name = i.Name,
                        IsExcludable = i.IsExcludable,
                        IsIncludable = i.IsIncludable
                    }).ToList(),
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
