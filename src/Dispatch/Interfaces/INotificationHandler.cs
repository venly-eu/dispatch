namespace Vesia.Dispatch;

/// <summary>
/// Handles a notification of type <typeparamref name="TNotification"/>.
/// A notification can have zero, one, or many handlers subscribed to it; all are invoked when the notification is published.
/// </summary>
/// <typeparam name="TNotification">The notification type being handled.</typeparam>
public interface INotificationHandler<in TNotification>
{
    /// <summary>Handles the specified notification.</summary>
    Task Handle(TNotification notification, CancellationToken cancellationToken = default);
}