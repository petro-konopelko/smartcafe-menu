using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Menus.ListMenus.Models;

namespace SmartCafe.Menu.Application.Features.Menus.ListMenus.Validators;

public class ListMenusQueryValidator : AbstractValidator<ListMenusQuery>
{
    public ListMenusQueryValidator()
    {
        RuleFor(x => x.CafeId)
            .NotEmpty().WithMessage(ValidationMessages.CafeIdRequired);
    }
}
