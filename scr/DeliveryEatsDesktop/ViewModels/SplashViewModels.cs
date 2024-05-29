using desktop.Service;

namespace desktop.ViewModels;

public class SplashViewModels : ViewModelBase
{
    public SplashViewModels(INotificationService notificationService) : base(notificationService)
    {
    }
}