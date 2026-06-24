namespace Vesia.Dispatch;

public interface IQueryHandler<in TQuery, TResult> : IHandler
{
    Task<TResult> Handle(TQuery query, CancellationToken cancellationToken = default);
}
