using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.DeleteMenu.Models;

namespace SmartCafe.Menu.Application.Features.Menus.DeleteMenu.Validators;

public class DeleteMenuCommandValidator : AbstractValidator<DeleteMenuCommand>
{
    public DeleteMenuCommandValidator()
    {
        RuleFor(x => x.CafeId)
            .NotEmpty().WithMessage(ValidationMessages.CafeIdRequired);

        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage(ValidationMessages.MenuIdRequired);
    }
}
