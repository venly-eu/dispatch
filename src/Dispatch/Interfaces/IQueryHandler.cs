namespace Vesia.Dispatch;

public interface IQueryHandler<in TQuery, TResult> : IHandler
    where TQuery : IQuery<TResult>
{
    Task<TResult> Handle(TQuery query, CancellationToken cancellationToken = default);
}
