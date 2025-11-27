using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.PublishMenu.Models;

namespace SmartCafe.Menu.Application.Features.Menus.PublishMenu.Validators;

public class PublishMenuCommandValidator : AbstractValidator<PublishMenuCommand>
{
    public PublishMenuCommandValidator()
    {
        RuleFor(x => x.CafeId)
            .NotEmpty().WithMessage(ValidationMessages.CafeIdRequired);

        RuleFor(x => x.MenuId)
            .NotEmpty().WithMessage(ValidationMessages.MenuIdRequired);
    }
}
