using FluentValidation;
using SmartCafe.Menu.Domain.ValueObjects;

namespace SmartCafe.Menu.Application.Features.Menus.Shared;

public class SectionDtoValidator : AbstractValidator<SectionDto>
{
    public SectionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Section name is required")
            .MaximumLength(100).WithMessage("Section name must not exceed 100 characters");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be greater than or equal to 0");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Section must have at least one item")
            .Must(items => items.Count <= 100)
            .WithMessage("Section cannot have more than 100 items");

        When(x => x.AvailableFrom.HasValue && x.AvailableTo.HasValue, () =>
        {
            RuleFor(x => x)
                .Must(x => x.AvailableFrom!.Value < x.AvailableTo!.Value)
                .WithMessage("AvailableFrom must be less than AvailableTo");
        });

        RuleForEach(x => x.Items).SetValidator(new MenuItemDtoValidator());
    }
}

public class MenuItemDtoValidator : AbstractValidator<MenuItemDto>
{
    public MenuItemDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required")
            .MaximumLength(200).WithMessage("Item name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Item description must not exceed 500 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.CategoryIds)
            .NotEmpty().WithMessage("Item must have at least one category");

        RuleFor(x => x.IngredientOptions)
            .Must(ingredients => ingredients == null || ingredients.Count <= 50)
            .WithMessage("Item cannot have more than 50 ingredient options");

        RuleForEach(x => x.IngredientOptions).SetValidator(new IngredientValidator());
    }
}

public class IngredientValidator : AbstractValidator<Ingredient>
{
    public IngredientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ingredient name is required")
            .MaximumLength(100).WithMessage("Ingredient name must not exceed 100 characters");

        RuleFor(x => x)
            .Must(x => x.IsExcludable || x.IsIncludable)
            .WithMessage("Ingredient must be either excludable or includable (or both)");
    }
}
