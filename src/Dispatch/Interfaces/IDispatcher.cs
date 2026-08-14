
namespace Vesia.Dispatch;

/// <summary>
/// Dispatches commands and queries to their registered handlers, and publishes notifications
/// to their subscribers.
/// </summary>
public interface IDispatcher
{
    /// <summary>Dispatches a command that returns a result.</summary>
    Task<TResult> DispatchAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);

    /// <summary>Dispatches a command with no return value.</summary>
    Task DispatchAsync(ICommand command, CancellationToken cancellationToken = default);

    /// <summary>Dispatches a query and returns its result.</summary>
    Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);

    /// <summary>Publishes a notification to all subscribed handlers.</summary>
    Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default);
}