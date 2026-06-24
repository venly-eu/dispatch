namespace Vesia.Dispatch.Exceptions;

public class HandlerNotFoundException : Exception
{
    public HandlerNotFoundException(string handlerName) 
        : base($"No handler registered for '{handlerName}'" +
               $"- Make sure you called AddDispatch() in the same project as your handlers.")
    { }
}