using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Validators;

/// <summary>
/// Base validator for MenuItemDto - validates format only (no ID validation)
/// </summary>
public class MenuItemDtoValidator : AbstractValidator<MenuItemDto>
{
    public MenuItemDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.ItemNameRequired)
            .MaximumLength(Domain.Entities.MenuItem.MaxNameLength).WithMessage(ValidationMessages.ItemNameMaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(Domain.Entities.MenuItem.MaxDescriptionLength).WithMessage(ValidationMessages.ItemDescriptionMaxLength);

        RuleFor(x => x.Price)
            .NotNull().WithMessage(ValidationMessages.PriceRequired)
            .SetValidator(new PriceDtoValidator());

        RuleForEach(x => x.Ingredients)
            .SetValidator(new IngredientValidator());
    }
}

/// <summary>
/// Validator for MenuItemDto in create scenarios - ID must be null
/// </summary>
public class MenuItemDtoForCreateValidator : MenuItemDtoValidator
{
    public MenuItemDtoForCreateValidator()
    {
        RuleFor(x => x.Id)
            .Null().WithMessage(ValidationMessages.ItemIdMustBeNullForCreate);
    }
}

/// <summary>
/// Validator for MenuItemDto in update scenarios - ID must not be null
/// </summary>
public class MenuItemDtoForUpdateValidator : MenuItemDtoValidator
{
    public MenuItemDtoForUpdateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(ValidationMessages.ItemIdRequired);
    }
}
