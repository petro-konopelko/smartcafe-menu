using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.ActivateMenu.Models;

namespace SmartCafe.Menu.Application.Features.Menus.ActivateMenu.Validators;

public class ActivateMenuCommandValidator : AbstractValidator<ActivateMenuCommand>
{
    public ActivateMenuCommandValidator()
    {
        RuleFor(x => x.CafeId)
            .NotEmpty().WithMessage(ValidationMessages.CafeIdRequired);

        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage(ValidationMessages.MenuIdRequired);
    }
}
