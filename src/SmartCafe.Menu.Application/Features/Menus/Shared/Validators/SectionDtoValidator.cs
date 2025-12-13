using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Validators;

/// <summary>
/// Base validator for SectionDto - validates format only (no ID validation)
/// </summary>
public class SectionDtoValidator : AbstractValidator<SectionDto>
{
    public SectionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.SectionNameRequired)
            .MaximumLength(100).WithMessage(ValidationMessages.SectionNameMaxLength);

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
    }
}

/// <summary>
/// Validator for SectionDto in create scenarios - ID must be null
/// </summary>
public class SectionDtoForCreateValidator : SectionDtoValidator
{
    public SectionDtoForCreateValidator()
    {
        RuleFor(x => x.Id)
            .Null().WithMessage(ValidationMessages.SectionIdMustBeNullForCreate);

        RuleForEach(x => x.Items).SetValidator(new MenuItemDtoForCreateValidator());
    }
}

/// <summary>
/// Validator for SectionDto in update scenarios - ID must not be null
/// </summary>
public class SectionDtoForUpdateValidator : SectionDtoValidator
{
    public SectionDtoForUpdateValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(ValidationMessages.SectionIdRequired);

        RuleForEach(x => x.Items).SetValidator(new MenuItemDtoForUpdateValidator());
    }
}
