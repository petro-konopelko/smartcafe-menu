using FluentValidation;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Validators;

public class SectionDtoValidator : AbstractValidator<SectionDto>
{
    public SectionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Section name is required")
            .MaximumLength(100).WithMessage("Section name must not exceed 100 characters");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order must be greater than or equal to 0");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Section must have at least one item")
            .Must(items => items.Count <= 100)
            .WithMessage("Section cannot have more than 100 items");

        When(x => x.AvailableFrom.HasValue && x.AvailableTo.HasValue, () =>
        {
            RuleFor(x => x)
                .Must(x => x.AvailableFrom!.Value < x.AvailableTo!.Value)
                .WithMessage("AvailableFrom must be less than AvailableTo");
        });

        RuleForEach(x => x.Items).SetValidator(new MenuItemDtoValidator());
    }
}
