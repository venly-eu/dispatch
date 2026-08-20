using Microsoft.Extensions.DependencyInjection;
using Vesia.Dispatch.Exceptions;

namespace Vesia.Dispatch;

internal sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
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