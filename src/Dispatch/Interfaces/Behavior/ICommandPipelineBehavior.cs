
namespace Vesia.Dispatch;

public interface ICommandPipelineBehavior<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<TResult> Handle(TCommand command, Func<Task<TResult>> next,
        CancellationToken cancellationToken = default);
}

public interface ICommandPipelineBehavior<in TCommand>
    where TCommand : ICommand
{
    Task Handle(TCommand command, Func<Task> next,
        CancellationToken cancellationToken = default);
}