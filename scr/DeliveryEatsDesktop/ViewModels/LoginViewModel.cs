using Avalonia.Controls.Notifications;
using desktop.Models;
using desktop.Service;
using ReactiveUI;
using Refit;
using System;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;
using Notification = Avalonia.Controls.Notifications.Notification;

namespace desktop.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly IAccessTokenRepository _accessTokenRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAuthorizationRepository _authorizationRepository;
    private readonly IViewNavigation _viewNavigation;
    private readonly ObservableAsPropertyHelper<bool> _isLogin;
    
    public bool IsLogin => _isLogin.Value;
    public Authorization AuthorizationData { get; }
    public ReactiveCommand<Unit, Unit> Login { get; }

    public LoginViewModel(IAccessTokenRepository accessTokenRepository, IRefreshTokenRepository refreshTokenRepository, 
        IAuthorizationRepository authorizationRepository,  INotificationService notificationService,
        IViewNavigation viewNavigation) : base(notificationService)
    {
        _accessTokenRepository = accessTokenRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _authorizationRepository = authorizationRepository;
        _viewNavigation = viewNavigation;

        Login = ReactiveCommand.CreateFromTask(LoginTask);
        Login.IsExecuting.ToProperty(this, a => a.IsLogin, out _isLogin);
        Login.ThrownExceptions.Subscribe(LoginThrownExc);
        AuthorizationData = new Authorization();
    }

    public async Task LoginTask()
    {
        if (string.IsNullOrEmpty(AuthorizationData.Login) && string.IsNullOrEmpty(AuthorizationData.Password))
        {
            _notificationService.ShowNotification(new Notification("Ошибка", "Заполните поля формы",
                NotificationType.Error));
            return;
        }
        Token token = await _authorizationRepository.LoginEmployee(AuthorizationData);
        await _refreshTokenRepository.AddRefreshToken(token.RefreshToken);
        _accessTokenRepository.AddAccessToken(token.AccessToken);
        _viewNavigation.GoToAndCloseCurrent<EmployeeViewModel>(this);
    }

    private void LoginThrownExc(Exception exp)
    {
        switch(exp)
        {
            case HttpRequestException:
                _notificationService.ShowNotification(new Notification("Ошибка",
                    "Не удалось установить соединение с сервером", NotificationType.Error));
            break;
            case ValidationApiException:
                if (((ValidationApiException)exp).StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    _notificationService.ShowNotification(new Notification("Ошибка", "Неверный логин или пароль",
                        NotificationType.Error));
                break;
        }
    }
}