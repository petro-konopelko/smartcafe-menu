namespace SmartCafe.Menu.Application.Mediation.Core;

/// <summary>
/// Base handler interface that unifies command and query handlers.
/// This enables type-safe handler resolution without reflection.
/// </summary>
public interface IHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}
