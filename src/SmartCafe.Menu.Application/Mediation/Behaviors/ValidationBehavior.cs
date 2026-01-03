using System.Collections.Concurrent;
using System.Reflection;
using FluentValidation;
using SmartCafe.Menu.Application.Mediation.Core;
using SmartCafe.Menu.Shared.Models;

namespace SmartCafe.Menu.Application.Mediation.Behaviors;

/// <summary>
/// Pipeline behavior that automatically validates requests using FluentValidation.
/// Works with handlers returning <see cref="Result"/> or <see cref="Result{T}"/>.
/// </summary>
internal sealed class ValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    private static readonly ConcurrentDictionary<Type, Func<Error, TResponse>> FailureResponseFactories
        = new();

    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
        {
            var error = Error.Validation(failures.Select(f => new ErrorDetail(
                f.ErrorMessage,
                f.ErrorCode,
                f.PropertyName)));

            return CreateValidationFailureResponse(error);
        }

        return await next();
    }

    private static TResponse CreateValidationFailureResponse(Error error)
    {
        var responseType = typeof(TResponse);

        var createdFactory = FailureResponseFactories.GetOrAdd(
            responseType,
            CreateFailureResponseFactory);

        return createdFactory(error);
    }

    private static Func<Error, TResponse> CreateFailureResponseFactory(Type responseType)
    {
        if (responseType == typeof(Result))
        {
            return error => (TResponse)(object)Result.Failure(error);
        }

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = responseType.GetMethod(
                    nameof(Result<>.Failure),
                    BindingFlags.Public | BindingFlags.Static,
                    [typeof(Error)])
                ?? throw new InvalidOperationException(
                    $"Could not find 'Failure' method on type '{responseType.FullName}'.");

            return failureMethod.CreateDelegate<Func<Error, TResponse>>();
        }

        throw new InvalidOperationException(
                $"Unsupported response type '{responseType.Name}'. " +
                $"ValidationBehavior only supports Result and Result<T>.");
    }
}
