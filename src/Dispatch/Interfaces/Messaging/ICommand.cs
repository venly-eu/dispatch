namespace Vesia.Dispatch;

/// <summary>
/// Marks a command that returns a result of type <typeparamref name="TResult"/>.
/// Implemented by command classes and paired with a matching <see cref="ICommandHandler{TCommand, TResult}"/>.
/// </summary>
/// <typeparam name="TResult">The type of result returned when this command is dispatched.</typeparam>
public interface ICommand<TResult> { }

/// <summary>
/// Marks a command with no return value.
/// Implemented by command classes and paired with a matching <see cref="ICommandHandler{TCommand}"/>.
/// </summary>
public interface ICommand { }