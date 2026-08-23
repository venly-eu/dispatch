using Microsoft.Extensions.DependencyInjection;
using Vesia.Dispatch.Exceptions;

namespace Vesia.Dispatch;

internal sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    public async Task<TResult> DispatchAsync<TResult>
        (ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        var handlerType = ServiceCollectionExtensions.CommandHandlerTypeCache.GetOrAdd(
            (command.GetType(), typeof(TResult)),
            static key => typeof(ICommandHandler<,>).MakeGenericType(key.Command, key.Result));
        
        dynamic handler = serviceProvider.GetService(handlerType) 
                          ?? throw new HandlerNotFoundException(command.GetType().Name);

        // resolve behaviors
        var behaviorType = ServiceCollectionExtensions.CommandBehaviorTypeCache.GetOrAdd(
            (command.GetType(), typeof(TResult)),
            static key => typeof(ICommandPipelineBehavior<,>).MakeGenericType(key.Command, key.Result));
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
        var handlerType = ServiceCollectionExtensions.VoidHandlerTypeCache.GetOrAdd(
            command.GetType(),
            static ct => typeof(ICommandHandler<>).MakeGenericType(ct));
    
        dynamic handler = serviceProvider.GetService(handlerType) 
                          ?? throw new HandlerNotFoundException(command.GetType().Name);

        // resolve behaviors
        var behaviorType = ServiceCollectionExtensions.VoidBehaviorTypeCache.GetOrAdd(
            command.GetType(),
            static ct => typeof(ICommandPipelineBehavior<>).MakeGenericType(ct));
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
        var handlerType = ServiceCollectionExtensions.QueryHandlerTypeCache.GetOrAdd(
            (query.GetType(), typeof(TResult)),
            static key => typeof(IQueryHandler<,>).MakeGenericType(key.Query, key.Result));
    
        dynamic handler = serviceProvider.GetService(handlerType) 
                          ?? throw new HandlerNotFoundException(query.GetType().Name);

        // resolve behaviors
        var behaviorType = ServiceCollectionExtensions.QueryBehaviorTypeCache.GetOrAdd(
            (query.GetType(), typeof(TResult)),
            static key => typeof(IQueryPipelineBehavior<,>).MakeGenericType(key.Query, key.Result));
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
    
    public async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);
        
        var handlerType = ServiceCollectionExtensions.NotificationHandlerTypeCache.GetOrAdd(
            notification.GetType(),
            static ct => typeof(INotificationHandler<>).MakeGenericType(ct));

        var handlers = serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            if (handler is null) continue;
            await ((dynamic)handler).Handle((dynamic)notification, cancellationToken);
        }
    }
}