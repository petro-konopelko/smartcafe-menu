using FluentValidation;
using SmartCafe.Menu.Application.Common.Validators;
using SmartCafe.Menu.Application.Features.Cafes.CreateCafe.Models;
using SmartCafe.Menu.Domain.Entities;

namespace SmartCafe.Menu.Application.Features.Cafes.CreateCafe;

public class CreateCafeCommandValidator : AbstractValidator<CreateCafeCommand>
{
    public CreateCafeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ValidationMessages.CafeNameRequired)
            .MaximumLength(Cafe.MaxNameLength).WithMessage(ValidationMessages.CafeNameTooLong);

        RuleFor(x => x.ContactInfo)
            .MaximumLength(Cafe.MaxContactInfoLength).WithMessage(ValidationMessages.CafeContactInfoTooLong)
            .When(x => !string.IsNullOrWhiteSpace(x.ContactInfo));
    }
}
