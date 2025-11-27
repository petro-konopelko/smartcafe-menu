using FluentValidation;
using SmartCafe.Menu.Domain.ValueObjects;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Validators;

public class IngredientValidator : AbstractValidator<Ingredient>
{
    public IngredientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ingredient name is required")
            .MaximumLength(100).WithMessage("Ingredient name must not exceed 100 characters");
    }
}
