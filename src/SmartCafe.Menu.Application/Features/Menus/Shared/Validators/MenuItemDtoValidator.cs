using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Validators;

public class MenuItemDtoValidator : AbstractValidator<MenuItemDto>
{
    public MenuItemDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.ItemNameRequired)
            .MaximumLength(200).WithMessage(ValidationMessages.ItemNameMaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage(ValidationMessages.ItemDescriptionMaxLength);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage(ValidationMessages.ItemPriceGreaterThanZero);

        RuleFor(x => x.CategoryIds)
            .NotEmpty().WithMessage(ValidationMessages.ItemMustHaveCategory)
            .Must(categories => categories.Count <= 10)
            .WithMessage(ValidationMessages.ItemMaxCategories);

        RuleForEach(x => x.IngredientOptions)
            .SetValidator(new IngredientValidator());
    }
}
