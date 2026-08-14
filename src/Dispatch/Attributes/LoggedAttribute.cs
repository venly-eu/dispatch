namespace Vesia.Dispatch;

/// <summary>
/// Marks a command as opted in to logging when <see cref="LoggingMode"/> is set to <see cref="LoggingMode.OptIn"/>.
/// Has no effect under <see cref="LoggingMode.All"/> or <see cref="LoggingMode.Disabled"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class LoggedAttribute : Attribute { }