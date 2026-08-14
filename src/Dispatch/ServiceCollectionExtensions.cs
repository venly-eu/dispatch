using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Vesia.Dispatch;

public static class ServiceCollectionExtensions
{
    private record HandlerTypes(Type InputArgument, Type ResultArgument, Type Handler);
    private record VoidHandlerTypes(Type InputArgument, Type Handler);
    private record NotificationTypes(Type InputArgument, Type Handler);

    extension (IServiceCollection services)
    {
        public IServiceCollection AddDispatch(
            Action<DispatchOptions>? configure = null,
            params Assembly[]? assemblies)
        {
            services.AddScoped<IDispatcher, Dispatcher>();
    
            if (assemblies is { Length: 0 } or null)
                assemblies = [Assembly.GetCallingAssembly()];
            
            var options = new DispatchOptions();
            configure?.Invoke(options);
            
            // Find CommandHandlers based on public, non-abstract, classes that implements the ICommandHandler interface
            // Select the Input/Output arguments plus the handler itself
            var commandHandlers = assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => t is { IsClass: true, IsAbstract: false } && (t.IsPublic || t.IsNestedPublic))
                .Where(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .Any(i => i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
                .SelectMany(handler => handler.GetInterfaces()
                    .Where(handlerInterface => 
                        handlerInterface.IsGenericType
                        && handlerInterface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
                    .Select(handlerInterface => new HandlerTypes(
                            InputArgument: handlerInterface.GetGenericArguments()[0],
                            ResultArgument: handlerInterface.GetGenericArguments()[1],
                            Handler: handler
                        )
                    )
                ).ToArray();
            
            // Find CommandHandlers that are void - No return type.
            // Based on public, non-abstract, classes that implements the ICommandHandler interface
            // Select the Input argument and the handler itself
            var voidCommandHandlers = assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => t is { IsClass: true, IsAbstract: false } && (t.IsPublic || t.IsNestedPublic))
                .Where(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .Any(i => i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)))
                .SelectMany(handler => handler.GetInterfaces()
                    .Where(handlerInterface => 
                        handlerInterface.IsGenericType
                        && handlerInterface.GetGenericTypeDefinition() == typeof(ICommandHandler<>))
                    .Select(handlerInterface => new VoidHandlerTypes(
                            InputArgument: handlerInterface.GetGenericArguments()[0],
                            Handler: handler
                        )
                    )
                ).ToArray();
            
            // Find QueryHandlers based on public, non-abstract, classes that implements the IQueryHandler interface
            // Select the Input/Output arguments plus the handler itself
            var queryHandlers = assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => t is { IsClass: true, IsAbstract: false } && (t.IsPublic || t.IsNestedPublic))
                .Where(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .Any(i => i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                .SelectMany(handler => handler.GetInterfaces()
                    .Where(handlerInterface => 
                        handlerInterface.IsGenericType
                        && handlerInterface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                    .Select(handlerInterface => new HandlerTypes(
                            InputArgument: handlerInterface.GetGenericArguments()[0],
                            ResultArgument: handlerInterface.GetGenericArguments()[1],
                            Handler: handler
                        )
                    )
                ).ToArray();
            
            // Find NotificationHandlers based on public, non-abstract, classes that implements the INotification interface
            // Select the Input arguments plus the handler itself
            var notificationHandlers = assemblies
                .SelectMany(a => a.GetExportedTypes())
                .Where(t => t is { IsClass: true, IsAbstract: false } && (t.IsPublic || t.IsNestedPublic))
                .Where(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .Any(i => i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)))
                .SelectMany(handler => handler.GetInterfaces()
                    .Where(handlerInterface => 
                        handlerInterface.IsGenericType
                        && handlerInterface.GetGenericTypeDefinition() == typeof(INotificationHandler<>))
                    .Select(handlerInterface => new NotificationTypes(
                            InputArgument: handlerInterface.GetGenericArguments()[0],
                            Handler: handler
                        )
                    )
                ).ToArray();
    
            // Register Commands as scoped based on the scanning
            foreach (var command in commandHandlers)
            {
                // Generate interface of CommandHandler based on the handlers input argument and its return type.
                var genericType = typeof(ICommandHandler<,>)
                    .MakeGenericType(command.InputArgument, command.ResultArgument);
                
                services.AddScoped(genericType, command.Handler);
                
                // Register behavior wrapper if logging enabled
                if (options.CommandLogging == LoggingMode.Disabled) continue;
                var behaviorType = options.CommandLogging == LoggingMode.All
                    ? typeof(CommandLoggingBehavior<,>)
                    : typeof(CommandOptInLoggingBehavior<,>);
                
                // Register Command-Pipeline interfaces 
                var behaviorInterface = typeof(ICommandPipelineBehavior<,>)
                    .MakeGenericType(command.InputArgument, command.ResultArgument);
                var behaviorService = behaviorType
                    .MakeGenericType(command.InputArgument, command.ResultArgument);
                
                services.AddScoped(behaviorInterface, behaviorService);
            }
            
            // Register void Commands as scoped
            foreach (var command in voidCommandHandlers)
            {
                // Generate Interfaces of void CommandHandlers based on handler's Input argument only
                var handlerInterface = typeof(ICommandHandler<>)
                    .MakeGenericType(command.InputArgument);
                
                services.AddScoped(handlerInterface, command.Handler);
                
                // Register behavior wrapper if logging enabled - if 'disabled' skip the steps below.
                if (options.CommandLogging == LoggingMode.Disabled) continue;
                var behaviorType = options.CommandLogging == LoggingMode.All
                    ? typeof(CommandLoggingBehavior<>)
                    : typeof(CommandOptInLoggingBehavior<>);
                
                var behaviorInterface = typeof(ICommandPipelineBehavior<>)
                    .MakeGenericType(command.InputArgument);
                var behaviorService = behaviorType
                    .MakeGenericType(command.InputArgument);
                
                services.AddScoped(behaviorInterface, behaviorService);
            }
            
            // Register Queries as scoped
            foreach (var query in queryHandlers)
            {
                // Generate interface of QueryHandler based on the handlers input argument and its return type.
                var handlerInterface = typeof(IQueryHandler<,>)
                    .MakeGenericType(query.InputArgument, query.ResultArgument);
                
                services.AddScoped(handlerInterface, query.Handler);
    
                // Register behavior wrapper if logging enabled - if 'disabled' skip the steps below.
                if (options.QueryLogging == LoggingMode.Disabled) continue;
                var behaviorType = options.QueryLogging == LoggingMode.All
                    ? typeof(QueryLoggingBehavior<,>)
                    : typeof(QueryOptInLoggingBehavior<,>);
    
                var behaviorInterface = typeof(IQueryPipelineBehavior<,>)
                    .MakeGenericType(query.InputArgument, query.ResultArgument);
                var behaviorService = behaviorType
                    .MakeGenericType(query.InputArgument, query.ResultArgument);
    
                services.AddScoped(behaviorInterface, behaviorService);
            }
            
            // Register Notifications as scoped
            foreach (var notification in notificationHandlers)
            {
                // Existing handler registration
                var genericType = typeof(INotificationHandler<>)
                    .MakeGenericType(notification.InputArgument);
                
                services.AddScoped(genericType, notification.Handler);
            }
            
            return services;
        }
    
        public IServiceCollection AddCommandBehavior<TBehavior>()
            where TBehavior : class
        {
            var behaviorInterface = typeof(TBehavior)
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && 
                                     i.GetGenericTypeDefinition() == typeof(ICommandPipelineBehavior<,>))
            ?? throw new InvalidOperationException($"{typeof(TBehavior).Name} does not implement ICommandPipelineBehavior<,>");
            
            // Register behavior
            services.AddScoped(behaviorInterface, typeof(TBehavior));
    
            return services;
        }
        
        public IServiceCollection AddQueryBehavior<TBehavior>()
            where TBehavior : class
        {
            var behaviorInterface = typeof(TBehavior)
                .GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && 
                                     i.GetGenericTypeDefinition() == typeof(IQueryPipelineBehavior<,>)) 
                                    ?? throw new InvalidOperationException($"{typeof(TBehavior).Name} does not implement IQueryPipelineBehavior<,>");
    
            // Register behavior
            services.AddScoped(behaviorInterface, typeof(TBehavior));
    
            return services;
        }
    }
}
