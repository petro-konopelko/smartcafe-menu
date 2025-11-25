using Microsoft.Extensions.DependencyInjection;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.Mediation;

public sealed class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>
    {
        ArgumentNullException.ThrowIfNull(request);

        // Type-safe handler resolution - no reflection!
        var handler = _serviceProvider.GetRequiredService<IHandler<TRequest, TResponse>>();

        // Get all pipeline behaviors for this request/response type
        var behaviors = _serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse();

        // Build the pipeline from behaviors, ending with the handler
        RequestHandlerDelegate<TResponse> handlerDelegate = () => handler.HandleAsync(request, cancellationToken);

        foreach (var behavior in behaviors)
        {
            var currentDelegate = handlerDelegate;
            handlerDelegate = () => behavior.HandleAsync(request, currentDelegate, cancellationToken);
        }

        return handlerDelegate();
    }
}
