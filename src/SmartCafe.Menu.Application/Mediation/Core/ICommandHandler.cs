namespace SmartCafe.Menu.Application.Mediation.Core;

public interface ICommandHandler<in TCommand, TResponse> : IHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}
