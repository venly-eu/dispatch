namespace Vesia.Dispatch.Exceptions;

/// <summary>
/// Thrown when no handler is registered for a dispatched command, query, or notification.
/// </summary>
public class HandlerNotFoundException : Exception
{
    /// <summary>Initializes a new instance with the name of the unresolved handler type.</summary>
    public HandlerNotFoundException(string handlerName) 
        : base($"No handler registered for '{handlerName}' " +
               $"- Make sure you called AddDispatch() in the same project as your handlers.")
    { }
}