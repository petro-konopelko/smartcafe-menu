using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Validators;

public class IngredientValidator : AbstractValidator<IngredientDto>
{
    public IngredientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.IngredientNameRequired)
            .MaximumLength(100).WithMessage(ValidationMessages.IngredientNameMaxLength);
    }
}
