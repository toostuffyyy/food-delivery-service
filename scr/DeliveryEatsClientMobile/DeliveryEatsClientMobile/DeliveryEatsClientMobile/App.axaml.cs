using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DeliveryEatsClientMobile.Context;
using DeliveryEatsClientMobile.Service;
using DeliveryEatsClientMobile.Views;
using Refit;
using Splat;
using MainViewModel = DeliveryEatsClientMobile.ViewModels.MainViewModel;

namespace DeliveryEatsClientMobile;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    public const string RestApiURL = "http://localhost:5221";

    public override void OnFrameworkInitializationCompleted()
    {
        Locator.CurrentMutable.Register(() => new SqLiteContext());
        Locator.CurrentMutable.Register(() => RestService.For<IProductRepository>(RestApiURL));
        Locator.CurrentMutable.Register(() => RestService.For<IAuthorizationRepository>(RestApiURL));
        Locator.CurrentMutable.Register(() => RestService.For<IOrderRepository>(RestApiURL));
        Locator.CurrentMutable.Register(() => RestService.For<ICategoryProductRepository>(RestApiURL));
        Locator.CurrentMutable.Register(() => RestService.For<IClientRepository>(RestApiURL));
        SplatRegistrations.Register<MainViewModel>();
        SplatRegistrations.RegisterLazySingleton<IRefreshTokenRepository, RefreshTokenRepository>();
        SplatRegistrations.RegisterLazySingleton<IAccessTokenRepository, AccessTokenRepository>();
        SplatRegistrations.RegisterLazySingleton<INotificationService, NotificationService>();
        SplatRegistrations.RegisterLazySingleton<IUpdateTokenService, UpdateTokenService>();
        SplatRegistrations.SetupIOC();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Locator.Current.GetService<MainViewModel>()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = Locator.Current.GetService<MainViewModel>()
            };
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}