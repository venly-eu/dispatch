namespace Vesia.Dispatch;

/// <summary>
/// Handles a query of type <typeparamref name="TQuery"/> and returns a result of type <typeparamref name="TResult"/>.
/// Implement this for each query to define how it's processed.
/// </summary>
/// <typeparam name="TQuery">The query type being handled.</typeparam>
/// <typeparam name="TResult">The type of result returned by the query.</typeparam>
public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    /// <summary>Handles the specified query and returns the result.</summary>
    Task<TResult> Handle(TQuery query, CancellationToken cancellationToken = default);
}