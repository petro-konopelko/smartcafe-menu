using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Models;

namespace SmartCafe.Menu.Application.Features.Menus.GetActiveMenu.Validators;

public class GetActiveMenuQueryValidator : AbstractValidator<GetActiveMenuQuery>
{
    public GetActiveMenuQueryValidator()
    {
        RuleFor(x => x.CafeId)
            .NotEmpty().WithMessage(ValidationMessages.CafeIdRequired);
    }
}
