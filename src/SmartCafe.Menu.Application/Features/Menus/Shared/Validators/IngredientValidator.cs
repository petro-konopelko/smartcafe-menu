using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Domain.ValueObjects;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Validators;

public class IngredientValidator : AbstractValidator<Ingredient>
{
    public IngredientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.IngredientNameRequired)
            .MaximumLength(100).WithMessage(ValidationMessages.IngredientNameMaxLength);
    }
}
