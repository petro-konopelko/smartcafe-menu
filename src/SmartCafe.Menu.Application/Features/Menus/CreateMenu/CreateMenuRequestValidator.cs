using FluentValidation;
using SmartCafe.Menu.Application.Features.Menus.Shared;

namespace SmartCafe.Menu.Application.Features.Menus.CreateMenu;

public class CreateMenuRequestValidator : AbstractValidator<CreateMenuRequest>
{
    public CreateMenuRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Menu name is required")
            .MaximumLength(200).WithMessage("Menu name must not exceed 200 characters");

        RuleFor(x => x.Sections)
            .NotEmpty().WithMessage("Menu must have at least one section");

        RuleForEach(x => x.Sections).SetValidator(new SectionDtoValidator());
    }
}
