using Avalonia.Controls.Notifications;

namespace desktop.Service
{
    public interface INotificationService
    {
        public void RegisterNotificationManager(INotificationManager notificationManager);
        public void ShowNotification(INotification notification);
    }
}
