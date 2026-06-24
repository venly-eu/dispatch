namespace Vesia.Dispatch;

public interface ICommandHandler<in TCommand, TResult> : IHandler
{
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default);
}
