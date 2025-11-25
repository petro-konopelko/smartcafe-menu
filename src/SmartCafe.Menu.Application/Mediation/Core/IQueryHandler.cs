namespace SmartCafe.Menu.Application.Mediation.Core;

public interface IQueryHandler<in TQuery, TResponse> : IHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}
