
namespace Vesia.Dispatch;

/// <summary>
/// Defines a pipeline behavior that wraps handling of a query returning <typeparamref name="TResult"/>.
/// Behaviors run around the handler and can execute logic before and/or after calling <paramref name="next"/>,
/// such as logging, validation, or caching. Logging is the only built-in behavior; add your own by
/// implementing this interface.
/// </summary>
/// <typeparam name="TQuery">The query type this behavior applies to.</typeparam>
/// <typeparam name="TResult">The type of result returned by the query.</typeparam>
public interface IQueryPipelineBehavior<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Handles the query, invoking <paramref name="next"/> to continue the pipeline
    /// (either the next behavior or the query handler itself).
    /// </summary>
    Task<TResult> Handle(TQuery query, Func<Task<TResult>> next,
        CancellationToken cancellationToken = default);
}