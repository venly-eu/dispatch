using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Vesia.Dispatch;

public class CommandOptInLoggingBehavior<TCommand, TResult>(ILogger<CommandOptInLoggingBehavior<TCommand, TResult>> logger)
    : ICommandPipelineBehavior<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    public async Task<TResult> Handle(TCommand command, Func<Task<TResult>> next, CancellationToken cancellationToken = default)
    {
        var shouldLog = typeof(TCommand).GetCustomAttribute<LoggedAttribute>() is not null;
        
        try
        {
            if (shouldLog)
                logger.LogInformation("Handling command {Command}", typeof(TCommand).Name);

            var result = await next();

            if (shouldLog)
                logger.LogInformation("Handled command {Command}", typeof(TCommand).Name);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling command {Command}", typeof(TCommand).Name);
            throw;
        }
    }
}

public class CommandOptInLoggingBehavior<TCommand>(ILogger<CommandOptInLoggingBehavior<TCommand>> logger)
    : ICommandPipelineBehavior<TCommand>
    where TCommand : ICommand
{
    public async Task Handle(TCommand command, Func<Task> next, CancellationToken cancellationToken = default)
    {
        var shouldLog = typeof(TCommand).GetCustomAttribute<LoggedAttribute>() is not null;
        
        try
        {
            if (shouldLog)
                logger.LogInformation("Handling command {Command}", typeof(TCommand).Name);

            await next();

            if (shouldLog)
                logger.LogInformation("Handled command {Command}", typeof(TCommand).Name);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling command {Command}", typeof(TCommand).Name);
            throw;
        }
    }
}