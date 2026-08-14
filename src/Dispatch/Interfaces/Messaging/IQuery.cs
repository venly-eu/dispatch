namespace Vesia.Dispatch;

/// <summary>
/// Marks a query that returns a result of type <typeparamref name="TResult"/>.
/// Implemented by query classes and paired with a matching <see cref="IQueryHandler{TQuery, TResult}"/>.
/// </summary>
/// <typeparam name="TResult">The type of result returned when this query is dispatched.</typeparam>
public interface IQuery<TResult> { }
