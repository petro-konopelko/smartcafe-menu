using FluentValidation;

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

public class SectionDtoValidator : AbstractValidator<SectionDto>
{
    public SectionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Section name is required")
            .MaximumLength(100).WithMessage("Section name must not exceed 100 characters");

        RuleFor(x => x.Items)
            .Must(items => items == null || items.Count <= 100)
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

public class MenuItemDtoValidator : AbstractValidator<MenuItemDto>
{
    public MenuItemDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Item name is required")
            .MaximumLength(200).WithMessage("Item name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Item description is required")
            .MaximumLength(500).WithMessage("Item description must not exceed 500 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.CategoryIds)
            .NotEmpty().WithMessage("Item must have at least one category");
    }
}
