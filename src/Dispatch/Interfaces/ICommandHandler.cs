namespace Vesia.Dispatch;

/// <summary>
/// Handles a command of type <typeparamref name="TCommand"/> and returns a result of type <typeparamref name="TResult"/>.
/// Implement this for each command that produces a result.
/// </summary>
/// <typeparam name="TCommand">The command type being handled.</typeparam>
/// <typeparam name="TResult">The type of result returned by the command.</typeparam>
public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    /// <summary>Handles the specified command and returns the result.</summary>
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Handles a command of type <typeparamref name="TCommand"/> with no return value.
/// Implement this for each command that does not produce a result.
/// </summary>
/// <typeparam name="TCommand">The command type being handled.</typeparam>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    /// <summary>Handles the specified command.</summary>
    Task Handle(TCommand command, CancellationToken cancellationToken = default);
}