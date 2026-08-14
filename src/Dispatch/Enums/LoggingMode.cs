namespace Vesia.Dispatch;

/// <summary>
/// Controls which commands get automatic logging via pipeline behaviors.
/// </summary>
public enum LoggingMode
{
    /// <summary>Log all Commands/Queries.</summary>
    All,

    /// <summary>No Commands/Queries are logged.</summary>
    Disabled,

    /// <summary>Only Commands/Queries explicitly opted in are logged.</summary>
    OptIn,
}