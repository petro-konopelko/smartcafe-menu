using FluentValidation;

namespace SmartCafe.Menu.Application.Features.Menus.CloneMenu;

public class CloneMenuRequestValidator : AbstractValidator<CloneMenuRequest>
{
    public CloneMenuRequestValidator()
    {
        RuleFor(x => x.NewMenuName)
            .NotEmpty().WithMessage("New menu name is required")
            .MaximumLength(200).WithMessage("Menu name must not exceed 200 characters");
    }
}
