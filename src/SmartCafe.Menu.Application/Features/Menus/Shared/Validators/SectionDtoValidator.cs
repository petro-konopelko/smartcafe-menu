using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Validators;

public class SectionDtoValidator : AbstractValidator<SectionDto>
{
    public SectionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.SectionNameRequired)
            .MaximumLength(100).WithMessage(ValidationMessages.SectionNameMaxLength);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.SectionDisplayOrderMinimum);

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage(ValidationMessages.SectionMustHaveItem)
            .Must(items => items.Count <= 100)
            .WithMessage(ValidationMessages.SectionMaxItems);

        When(x => x.AvailableFrom.HasValue && x.AvailableTo.HasValue, () =>
        {
            RuleFor(x => x)
                .Must(x => x.AvailableFrom!.Value < x.AvailableTo!.Value)
                .WithMessage(ValidationMessages.SectionAvailableFromLessThanTo);
        });

        RuleForEach(x => x.Items).SetValidator(new MenuItemDtoValidator());
    }
}
