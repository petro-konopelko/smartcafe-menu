using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.CreateMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Validators;

namespace SmartCafe.Menu.Application.Features.Menus.CreateMenu.Validators;

public class CreateMenuRequestValidator : AbstractValidator<CreateMenuRequest>
{
    public CreateMenuRequestValidator()
    {
        RuleFor(x => x.CafeId)
            .NotEmpty().WithMessage(ValidationMessages.CafeIdRequired);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.MenuNameRequired)
            .MaximumLength(200).WithMessage(ValidationMessages.MenuNameMaxLength);

        RuleFor(x => x.Sections)
            .NotEmpty().WithMessage(ValidationMessages.MenuMustHaveSection);

        RuleForEach(x => x.Sections).SetValidator(new SectionDtoValidator());
    }
}
