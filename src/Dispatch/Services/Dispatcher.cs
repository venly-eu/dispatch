using Microsoft.Extensions.DependencyInjection;
using Vesia.Dispatch.Exceptions;

namespace Vesia.Dispatch;

/// <summary>
/// Dispatches commands and queries to their handlers via reflection-based DI resolution.
/// Handlers are matched to commands/queries by closing the corresponding generic handler
/// interface with the command/query's concrete type (and result type, where applicable).
/// This link is not compile-time checked — a missing or mismatched handler registration
/// throws <see cref="HandlerNotFoundException"/> at dispatch time.
/// </summary>
public class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    /// <summary>Dispatches a command that returns a result.</summary>
    public async Task<TResult> DispatchAsync<TResult>
        (ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<,>)
            .MakeGenericType(command.GetType(), typeof(TResult));
        
        dynamic handler = serviceProvider.GetService(handlerType) 
                          ?? throw new HandlerNotFoundException(command.GetType().Name);

        // resolve behaviors
        var behaviorType = typeof(ICommandPipelineBehavior<,>)
            .MakeGenericType(command.GetType(), typeof(TResult));
        var behaviors = serviceProvider.GetServices(behaviorType);

        // build chain
        var next = (Func<Task<TResult>>)(() => handler.Handle((dynamic)command, cancellationToken));
        foreach (var behavior in behaviors.Reverse())
        {
            if (behavior is null) continue;
            
            var currentNext = next;
            next = () => ((dynamic)behavior).Handle((dynamic)command, currentNext, cancellationToken);
        }

        return await next();
    }
    
    /// <summary>Dispatches a command with no return value.</summary>
    public async Task DispatchAsync
        (ICommand command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<>)
            .MakeGenericType(command.GetType());
        
        dynamic handler = serviceProvider.GetService(handlerType) 
                          ?? throw new HandlerNotFoundException(command.GetType().Name);

        // resolve behaviors
        var behaviorType = typeof(ICommandPipelineBehavior<>)
            .MakeGenericType(command.GetType());
        var behaviors = serviceProvider.GetServices(behaviorType);

        // build chain
        var next = (Func<Task>)(() => handler.Handle((dynamic)command, cancellationToken));
        foreach (var behavior in behaviors.Reverse())
        {
            if (behavior is null) continue;
            
            var currentNext = next;
            next = () => ((dynamic)behavior).Handle((dynamic)command, currentNext, cancellationToken);
        }
        
        await next();
    }

    /// <summary>Dispatches a query and returns its result.</summary>
    public async Task<TResult> DispatchAsync<TResult>
        (IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IQueryHandler<,>)
            .MakeGenericType(query.GetType(), typeof(TResult));
        
        dynamic handler = serviceProvider.GetService(handlerType) 
                          ?? throw new HandlerNotFoundException(query.GetType().Name);

        // resolve behaviors
        var behaviorType = typeof(IQueryPipelineBehavior<,>)
            .MakeGenericType(query.GetType(), typeof(TResult));
        var behaviors = serviceProvider.GetServices(behaviorType);

        // build chain
        var next = (Func<Task<TResult>>)(() => handler.Handle((dynamic)query, cancellationToken));
        foreach (var behavior in behaviors.Reverse())
        {
            if (behavior is null) continue;
            
            var currentNext = next;
            next = () => ((dynamic)behavior).Handle((dynamic)query, currentNext, cancellationToken);
        }

        return await next();
    }

    /// <summary>Dispatches an event to every handler that subscribes to it. Returns nothing</summary>
    public async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(INotificationHandler<>)
            .MakeGenericType(notification!.GetType());

        var handlers = serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            if (handler is null) continue;
            await ((dynamic)handler).Handle((dynamic)notification, cancellationToken);
        }
    }
}