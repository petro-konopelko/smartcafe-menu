using FluentValidation;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Domain.Common;

namespace SmartCafe.Menu.Application.Mediation.Behaviors;

/// <summary>
/// Pipeline behavior that automatically validates requests using FluentValidation.
/// /// Works with handlers returning <see cref="Result"/> or <see cref="Result{T}"/>.
/// </summary>
public sealed class ValidationBehavior<TRequest, T> : IPipelineBehavior<TRequest, Result<T>>
    where TRequest : IRequest<Result<T>>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<Result<T>> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<Result<T>> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var error = Error.Validation(failures.Select(f => new ErrorDetail(
                f.ErrorMessage,
                f.ErrorCode,
                f.PropertyName)));

            return Result<T>.Failure(error);
        }

        return await next();
    }
}
