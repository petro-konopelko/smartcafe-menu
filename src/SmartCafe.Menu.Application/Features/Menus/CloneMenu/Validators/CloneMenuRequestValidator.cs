using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.CloneMenu.Models;

namespace SmartCafe.Menu.Application.Features.Menus.CloneMenu.Validators;

public class CloneMenuRequestValidator : AbstractValidator<CloneMenuRequest>
{
    public CloneMenuRequestValidator()
    {
        RuleFor(x => x.CafeId)
            .NotEmpty().WithMessage(ValidationMessages.CafeIdRequired);

        RuleFor(x => x.SourceMenuId)
            .NotEmpty().WithMessage(ValidationMessages.SourceMenuIdRequired);

        RuleFor(x => x.NewName)
            .NotEmpty().WithMessage(ValidationMessages.MenuNameRequired)
            .MaximumLength(200).WithMessage(ValidationMessages.MenuNameMaxLength);
    }
}
