namespace SmartCafe.Menu.Application.Mediation.Core;

public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
