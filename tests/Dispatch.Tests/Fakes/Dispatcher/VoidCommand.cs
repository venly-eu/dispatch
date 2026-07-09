
namespace Vesia.Dispatch.Tests.Fakes.Dispatcher;

public class CallTracker
{
    public bool WasCalled { get; set; }
}

public record VoidCommand : ICommand;

public class VoidCommandHandler(CallTracker tracker) 
    : ICommandHandler<VoidCommand>
{
    public Task Handle(VoidCommand command, CancellationToken cancellationToken = default)
    {
        tracker.WasCalled = true;
        return Task.CompletedTask;
    }
}