using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Cafes.GetCafe.Models;

namespace SmartCafe.Menu.Application.Features.Cafes.GetCafe;

public class GetCafeQueryValidator : AbstractValidator<GetCafeQuery>
{
    public GetCafeQueryValidator()
    {
        RuleFor(x => x.CafeId)
            .NotEmpty().WithMessage(ValidationMessages.CafeIdRequired);
    }
}
