
namespace Vesia.Dispatch;

public interface IQueryPipelineBehavior<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<TResult> Handle(TQuery query, Func<Task<TResult>> next,
        CancellationToken cancellationToken = default);
}
