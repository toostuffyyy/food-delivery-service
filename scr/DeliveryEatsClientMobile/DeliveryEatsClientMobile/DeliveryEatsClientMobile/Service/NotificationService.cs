using Avalonia.Controls.Notifications;

namespace DeliveryEatsClientMobile.Service
{
    public class NotificationService : INotificationService
    {
        private INotificationManager? _notificationManager;

        public void RegisterNotificationManager(INotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

        public void ShowNotification(INotification notification)
        {
            _notificationManager?.Show(notification);
        }
    }
}
