namespace SmartCafe.Menu.Application.Mediation.Core;

public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
