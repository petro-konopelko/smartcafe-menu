using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.CloneMenu.Models;

namespace SmartCafe.Menu.Application.Features.Menus.CloneMenu.Validators;

public class CloneMenuCommandValidator : AbstractValidator<CloneMenuCommand>
{
    public CloneMenuCommandValidator()
    {
        RuleFor(x => x.CafeId)
            .NotEmpty().WithMessage(ValidationMessages.CafeIdRequired);

        RuleFor(x => x.SourceMenuId)
            .NotEmpty().WithMessage(ValidationMessages.SourceMenuIdRequired);

        RuleFor(x => x.NewName)
            .NotEmpty().WithMessage(ValidationMessages.MenuNameRequired)
            .MaximumLength(Domain.Entities.Menu.MaxNameLength).WithMessage(ValidationMessages.MenuNameMaxLength);
    }
}
