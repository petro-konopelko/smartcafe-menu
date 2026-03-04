using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.Shared.Models;

namespace SmartCafe.Menu.Application.Features.Menus.Shared.Validators;

public class PriceDtoValidator : AbstractValidator<PriceDto>
{
    public PriceDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage(ValidationMessages.ItemPriceGreaterThanZero);

        RuleFor(x => x.Unit)
            .IsInEnum().WithMessage(ValidationMessages.PriceUnitInvalid);

        RuleFor(x => x.DiscountPercent)
            .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.DiscountPercentInvalid)
            .LessThan(100).WithMessage(ValidationMessages.DiscountPercentInvalid);
    }
}
