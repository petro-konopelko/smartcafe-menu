using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SmartCafe.Menu.Application.Mediation;
using SmartCafe.Menu.Application.Mediation.Behaviors;
using SmartCafe.Menu.Application.Mediation.Core;

namespace SmartCafe.Menu.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // Register Mediator
        services.AddScoped<IMediator, Mediator>();

        // Register pipeline behaviors
        // ValidationBehavior handles both Result and Result<T> as TResponse
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Auto-register all handlers using Scrutor
        services.Scan(scan => scan
            .FromAssemblies(typeof(ServiceCollectionExtensions).Assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // Auto-register all validators using Scrutor
        services.Scan(scan => scan
            .FromAssemblies(typeof(ServiceCollectionExtensions).Assembly)
            .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
