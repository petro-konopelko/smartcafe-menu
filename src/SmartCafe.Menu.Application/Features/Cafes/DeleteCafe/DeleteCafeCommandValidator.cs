using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Cafes.DeleteCafe.Models;

namespace SmartCafe.Menu.Application.Features.Cafes.DeleteCafe;

public class DeleteCafeCommandValidator : AbstractValidator<DeleteCafeCommand>
{
    public DeleteCafeCommandValidator()
    {
        RuleFor(x => x.CafeId)
            .NotEmpty().WithMessage(ValidationMessages.CafeIdRequired);
    }
}
