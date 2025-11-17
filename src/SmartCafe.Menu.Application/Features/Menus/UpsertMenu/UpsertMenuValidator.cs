using FluentValidation;

namespace SmartCafe.Menu.Application.Features.Menus.UpsertMenu;

public class UpsertMenuValidator : AbstractValidator<UpsertMenuRequest>
{
    public UpsertMenuValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.Sections)
            .NotEmpty()
            .WithMessage("Menu must have at least one section");

        RuleForEach(x => x.Sections)
            .SetValidator(new UpsertSectionValidator());
    }
}

public class UpsertSectionValidator : AbstractValidator<UpsertSectionDto>
{
    public UpsertSectionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Section must have at least one item")
            .Must(items => items.Count <= 100)
            .WithMessage("Section cannot have more than 100 items");

        RuleForEach(x => x.Items)
            .SetValidator(new UpsertMenuItemValidator());
    }
}

public class UpsertMenuItemValidator : AbstractValidator<UpsertMenuItemDto>
{
    public UpsertMenuItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be positive");

        RuleFor(x => x.CategoryIds)
            .NotEmpty()
            .WithMessage("Item must have at least one category");

        RuleForEach(x => x.Ingredients)
            .SetValidator(new IngredientValidator());
    }
}

public class IngredientValidator : AbstractValidator<IngredientDto>
{
    public IngredientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);
    }
}
