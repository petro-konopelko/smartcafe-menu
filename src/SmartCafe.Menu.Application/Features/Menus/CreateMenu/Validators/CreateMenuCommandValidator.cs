using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.CreateMenu.Models;
using SmartCafe.Menu.Application.Features.Menus.Shared.Validators;

namespace SmartCafe.Menu.Application.Features.Menus.CreateMenu.Validators;

public class CreateMenuCommandValidator : AbstractValidator<CreateMenuCommand>
{
    public CreateMenuCommandValidator()
    {
        RuleFor(x => x.CafeId)
            .NotEmpty().WithMessage(ValidationMessages.CafeIdRequired);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.MenuNameRequired)
            .MaximumLength(Domain.Entities.Menu.MaxNameLength).WithMessage(ValidationMessages.MenuNameMaxLength);

        RuleFor(x => x.Sections)
            .NotEmpty().WithMessage(ValidationMessages.MenuMustHaveSection);

        RuleForEach(x => x.Sections).SetValidator(new SectionDtoForCreateValidator());
    }
}
