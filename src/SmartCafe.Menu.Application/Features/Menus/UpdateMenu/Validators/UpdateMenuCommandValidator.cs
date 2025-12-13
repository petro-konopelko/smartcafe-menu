using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.Shared.Validators;
using SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Models;

namespace SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Validators;

public class UpdateMenuCommandValidator : AbstractValidator<UpdateMenuCommand>
{
    public UpdateMenuCommandValidator()
    {
        RuleFor(x => x.CafeId)
            .NotEmpty().WithMessage(ValidationMessages.CafeIdRequired);

        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage(ValidationMessages.MenuIdRequired);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.MenuNameRequired)
            .MaximumLength(200).WithMessage(ValidationMessages.MenuNameMaxLength);

        RuleFor(x => x.Sections)
            .NotEmpty().WithMessage(ValidationMessages.MenuMustHaveSection);

        RuleForEach(x => x.Sections).SetValidator(new SectionDtoForUpdateValidator());
    }
}
