
namespace Vesia.Dispatch;

/// <summary>
/// Configuration options for Dispatch, set when calling <c>AddDispatch()</c>.
/// </summary>
public class DispatchOptions
{
    /// <summary>
    /// Controls logging behavior for commands. Defaults to <see cref="LoggingMode.OptIn"/>,
    /// meaning only commands marked with <see cref="LoggedAttribute"/> are logged.
    /// </summary>
    public LoggingMode CommandLogging { get; set; } = LoggingMode.OptIn;

    /// <summary>
    /// Controls logging behavior for queries. Defaults to <see cref="LoggingMode.OptIn"/>,
    /// meaning only queries marked with <see cref="LoggedAttribute"/> are logged.
    /// </summary>
    public LoggingMode QueryLogging { get; set; } = LoggingMode.OptIn;
}