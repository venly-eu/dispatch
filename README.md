# What is Vesia.Dispatch?
Vesia.Dispatch is a free, open-source alternative to MediatR built for CQRS patterns and Clean Architecture.

There are 3 types that can be dispatched — Commands, Queries, and Notifications.

Logging is built in and flexible. You can log all commands and queries, disable logging entirely, or use Opt-In mode to only log types marked with `[Logged]`. Commands and queries are configured independently.

Vesia.Dispatch uses the standard `Microsoft.Extensions.Logging` abstraction — plug in any logging provider you prefer (Serilog, NLog, etc.) and it works automatically.

> [!WARNING]
> **Assembly Registration**
>
> `Vesia.Dispatch` scans the assembly you provide at registration time to discover handlers. Make sure you call `AddDispatch` from the project that contains your commands, queries, and handlers — typically your **Application layer** — not from a referencing project like your API or host.
>
> Registering from the wrong assembly will result in a `HandlerNotFoundException` at runtime.

# The types
| Type | Returns | Handlers | Use case |
|------|---------|----------|----------|
| Command | `TResult` | Exactly one | Write operations, state changes |
| Query | `TResult` | Exactly one | Read operations, data retrieval |
| Notification | `void` | Zero or more | Domain events, fan-out |


# Getting started

## Installation
```bash
dotnet add package Vesia.Dispatch
```

### Setup
```csharp
// Remember to call this in the same asembly as your queries/commands and their handlers
services.AddDispatch(options =>
{
    options.CommandLogging = LoggingMode.All;
    options.QueryLogging = LoggingMode.OptIn;
});
```

### Commands
```csharp
// Define a command with a return
public record CreateUserCommand(string Name) : ICommand<Guid>;

// Define a handler
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = new User(command.Name);

        return user.Uid;
    }
}

// Dispatch it
var user = await dispatcher.DispatchAsync(new CreateUserCommand("Oliver"));
```

### Queries
```csharp
// Define a query with a return type
public record GetUserQuery(Guid Id) : IQuery<UserDto>;

// Define a handler for the query by adding the query + return type
public class GetUserQueryHandler : IQueryHandler<GetUserQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(query.Id); 
        return new UserDto(user.Id, user.Name);
    }
}

// Dispatch it
var user = await dispatcher.DispatchAsync(new GetUserQuery(id));
```

### Notifications
```csharp
// Define a notification - No interface needed as long as it is a record and
// the NotificationHandler Handle() includes it in its parameter 
public record UserCreatedEvent(Guid UserId);

// Define a handler (multiple allowed)
public class SendWelcomeEmailHandler : INotificationHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        // send welcome email
    }
}

// Publish it
await dispatcher.PublishAsync(new UserCreatedEvent(user.Id));
```

### Opt-In Logging
```csharp
// Mark specific queries for logging when using LoggingMode.OptIn
[Logged]
public record GetUserQuery(Guid Id) : IQuery<UserDto>;
```

### Custom Pipeline Behaviors
You can add your own pipeline behaviors for cross-cutting concerns like validation or transactions.

```csharp
public class ValidationBehavior : ICommandPipelineBehavior<CreateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(CreateUserCommand command, Func<Task<UserDto>> next, CancellationToken cancellationToken)
    {
        // validate before
        var result = await next();
        // do something after
        return result;
    }
}
```

Register it in `Program.cs`:

```csharp
services.AddCommandBehavior<ValidationBehavior>();
```
