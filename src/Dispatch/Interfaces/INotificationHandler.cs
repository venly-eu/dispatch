namespace Vesia.Dispatch;

public interface INotificationHandler<in TNotification>
{
    Task Handle(TNotification notification, CancellationToken cancellationToken = default);
}