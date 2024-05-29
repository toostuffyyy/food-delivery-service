using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using desktop.Context;
using desktop.Service;
using desktop.ViewModels;
using Refit;
using Splat;
using System;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using desktop.Services;

namespace desktop.Views;

public partial class SplashWindow : Window
{
    public const string RestApiURL = "http://localhost:5221";
    public SplashWindow()
    {
        InitializeComponent();
        Dispatcher.UIThread.Post(() => LoadApp(), DispatcherPriority.Background);
    }
    private async Task LoadApp()
    {
        var statusTextBlock = this.FindControl<TextBlock>("StatusTextBlock");
        statusTextBlock.Text = "Регистрация сервисов";
        await Task.Run(async () =>
        {
            Locator.CurrentMutable.RegisterLazySingleton(() => new SqLiteContext());
            Locator.CurrentMutable.Register(() => RestService.For<IEmployeeRepository>(RestApiURL));
            Locator.CurrentMutable.Register(() => RestService.For<IOrderRepository>(RestApiURL));
            Locator.CurrentMutable.Register(() => RestService.For<ICategoryProductRepository>(RestApiURL));
            Locator.CurrentMutable.Register(() => RestService.For<IImageService>(RestApiURL));
            Locator.CurrentMutable.Register(() => RestService.For<IStatusRepository>(RestApiURL));
            Locator.CurrentMutable.Register(() => RestService.For<IRoleRepository>(RestApiURL));
            Locator.CurrentMutable.Register(() => RestService.For<IStatusPayRepository>(RestApiURL));
            Locator.CurrentMutable.Register(() => RestService.For<IStatisticRepository>(RestApiURL));
            Locator.CurrentMutable.Register<IDialogService>(() => new DialogService());
            Locator.CurrentMutable.RegisterLazySingleton(() => RestService.For<IAuthorizationRepository>(RestApiURL));
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                SplatRegistrations.RegisterLazySingleton<IRefreshTokenRepository, RefreshTokenWindowsRepository>();
            else throw new PlatformNotSupportedException();
            SplatRegistrations.Register<LoginViewModel>();
            SplatRegistrations.Register<EmployeeViewModel>();
            SplatRegistrations.Register<AddEditViewModel>();
            SplatRegistrations.RegisterLazySingleton<IAccessTokenRepository, AccessTokenRepository>();
            SplatRegistrations.RegisterLazySingleton<IViewNavigation, WindowsNavigation>();
            SplatRegistrations.RegisterLazySingleton<INotificationService, NotificationService>();
            SplatRegistrations.RegisterLazySingleton<IUpdateTokenService, UpdateTokenService>();
            SplatRegistrations.RegisterLazySingleton<IFilePickerService, FilePickerService>();
            SplatRegistrations.RegisterLazySingleton<IReportProductService, ProductReportService>();
            SplatRegistrations.SetupIOC();
        });
        statusTextBlock.Text = "Попытка авторизации через токен";
        string? token = await Task<string?>.Run(() =>
        {
            var refreshTokenRepository = Locator.Current.GetService<IRefreshTokenRepository>();
            return refreshTokenRepository.GetRefreshToken();
        });
        if (token == null)
        {
            Locator.Current.GetService<IViewNavigation>().GoToAndCloseCurrent<LoginViewModel>((ViewModelBase)DataContext);
            return;
        }
        try
        {
            var newTokens = await Locator.Current.GetService<IAuthorizationRepository>().UpdateTokenEmployee(token);
            await Locator.Current.GetService<IRefreshTokenRepository>().UpdateRefreshToken(newTokens.RefreshToken);
            Locator.Current.GetService<IAccessTokenRepository>().AddAccessToken(newTokens.AccessToken);
            statusTextBlock.Text = "Входим в систему";
            Locator.Current.GetService<IViewNavigation>().GoToAndCloseCurrent<EmployeeViewModel>((ViewModelBase)DataContext);
        }
        catch (HttpRequestException)
        {
            statusTextBlock.Text = "Ошибка со стороны сервера";
            await Task.Delay(3000);
            Close();
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                statusTextBlock.Text = "Ошибка со стороны сервера";
                await Task.Delay(3000);
                this.Close();
                return;
            }
            await Locator.Current.GetService<IRefreshTokenRepository>().DeleteRefreshToken();
            Locator.Current.GetService<IViewNavigation>().GoToAndCloseCurrent<LoginViewModel>((ViewModelBase)DataContext);
        }
    }
}