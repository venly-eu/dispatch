
namespace Vesia.Dispatch;

/// <summary>
/// Defines a pipeline behavior that wraps handling of a command returning <typeparamref name="TResult"/>.
/// Behaviors run around the handler and can execute logic before and/or after calling <paramref name="next"/>,
/// such as logging, validation, or transaction management.
/// </summary>
/// <typeparam name="TCommand">The command type this behavior applies to.</typeparam>
/// <typeparam name="TResult">The type of result returned by the command.</typeparam>
public interface ICommandPipelineBehavior<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Handles the command, invoking <paramref name="next"/> to continue the pipeline
    /// (either the next behavior or the command handler itself).
    /// </summary>
    Task<TResult> Handle(TCommand command, Func<Task<TResult>> next,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Defines a pipeline behavior that wraps handling of a command with no return value.
/// Behaviors run around the handler and can execute logic before and/or after calling <paramref name="next"/>,
/// such as logging, validation, or transaction management.
/// </summary>
/// <typeparam name="TCommand">The command type this behavior applies to.</typeparam>
public interface ICommandPipelineBehavior<in TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Handles the command, invoking <paramref name="next"/> to continue the pipeline
    /// (either the next behavior or the command handler itself).
    /// </summary>
    Task Handle(TCommand command, Func<Task> next,
        CancellationToken cancellationToken = default);
}