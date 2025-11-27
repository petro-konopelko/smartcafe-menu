using FluentValidation;
using SmartCafe.Menu.Application.Features.Menus.Shared.Validators;
using SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Models;

namespace SmartCafe.Menu.Application.Features.Menus.UpdateMenu.Validators;

public class UpdateMenuRequestValidator : AbstractValidator<UpdateMenuRequest>
{
    public UpdateMenuRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Menu name is required")
            .MaximumLength(200).WithMessage("Menu name must not exceed 200 characters");

        RuleFor(x => x.Sections)
            .NotEmpty().WithMessage("Menu must have at least one section");

        RuleForEach(x => x.Sections).SetValidator(new SectionDtoValidator());
    }
}
