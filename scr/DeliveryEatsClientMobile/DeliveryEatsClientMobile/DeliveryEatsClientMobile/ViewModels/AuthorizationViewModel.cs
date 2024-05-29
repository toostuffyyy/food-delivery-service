using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using DeliveryEatsClientMobile.Models;
using DeliveryEatsClientMobile.Service;
using ReactiveUI;
using Refit;
using Authorization = DeliveryEatsClientMobile.Models.Authorization;
using Notification = Avalonia.Controls.Notifications.Notification;

namespace DeliveryEatsClientMobile.ViewModels;

public class AuthorizationViewModel : ViewModelBase
{
    private readonly IAccessTokenRepository _accessTokenRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IAuthorizationRepository _authorizationRepository;
    
    private readonly ObservableAsPropertyHelper<bool> _isCheckedClient;
    private bool _insertLogin;
    private bool _isCheckedEmail = true;
    private string _textView = "Войдите в профиль";

    public bool IsCheckedClient => _isCheckedClient.Value;
    public bool InsertLogin
    {
        get => _insertLogin;
        set => this.RaiseAndSetIfChanged(ref _insertLogin, value);
    }
    public bool IsCheckedEmail
    {
        get => _isCheckedEmail;
        set => this.RaiseAndSetIfChanged(ref _isCheckedEmail, value);
    }
    public string TextView
    {
        get => _textView;
        set => this.RaiseAndSetIfChanged(ref _textView, value);
    }
    public Authorization Authorization { get; }
    public ClientDetails Client { get; set; }
    
    public ReactiveCommand<Unit, bool> CheckClientCommand { get; }
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> AddClientCommand { get; }
    public ReactiveCommand<Unit, Unit> GoBackCommand { get; }

    public AuthorizationViewModel(IAccessTokenRepository accessTokenRepository, IRefreshTokenRepository refreshTokenRepository,
        IAuthorizationRepository authorizationRepository, INotificationService notificationService,
        IUpdateTokenService updateTokenService, IClientRepository clientRepository, Order order) : base(notificationService, updateTokenService, order)
    {
        _accessTokenRepository = accessTokenRepository;
        _authorizationRepository = authorizationRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _clientRepository = clientRepository;
        
        Authorization = new Authorization();
        Client = new ClientDetails { Addresses = new List<Address> { new() } };
        // Вход.
        LoginCommand = ReactiveCommand.CreateFromTask(Login);
        LoginCommand.ThrownExceptions.Subscribe(LoginThrownExc);
        // Регистрация.
        AddClientCommand = ReactiveCommand.CreateFromTask(AddClient);
        AddClientCommand.ThrownExceptions.Subscribe(LoginThrownExc);
        // Переход и проверка на существование клиента.
        CheckClientCommand = ReactiveCommand.CreateFromTask(CheckClient);
        CheckClientCommand.ThrownExceptions.Subscribe(LoginThrownExc);
        _isCheckedClient = CheckClientCommand.ToProperty(this, a => a.IsCheckedClient);
        // Переход обратно.
        GoBackCommand = ReactiveCommand.CreateFromTask(GoBack);
        GoBackCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GoBackCommand));
    }

    private async Task<bool> CheckClient()
    {
        if (!string.IsNullOrEmpty(Client.Email) || !string.IsNullOrEmpty(Client.PhoneNumber))
        {
            var response = await _authorizationRepository
                .CheckClient((IsCheckedEmail 
                    ? Client.Email 
                    : Client.PhoneNumber?.Replace(" ",""))!);
            
            InsertLogin = true;
            TextView = response ? "Введите пароль" : "Введите данные";
            return response;
        }
        else
            _notificationService.ShowNotification(new Notification(
                "Предупреждение", "Нужно ввести почту или телефон", NotificationType.Warning));
        return false;
    }
    private async Task AddClient()
    {
        Client.PhoneNumber = Client.PhoneNumber?.Replace(" ", "");
        Authorization.Login = IsCheckedEmail ? Client.Email : Client.PhoneNumber;
        Authorization.Password = Client.Password;
        
        await _clientRepository.AddClient(Client);
        var token = await _authorizationRepository.LoginClient(Authorization);
        await _refreshTokenRepository.AddRefreshToken(token.RefreshToken);
        _accessTokenRepository.AddAccessToken(token.AccessToken);
        
        var mainViewModel = Owner.Owner as MainViewModel ?? Owner.Owner.Owner as MainViewModel;
        mainViewModel.SelectedViewModel = Owner;
        (mainViewModel.SelectedViewModel as ProductsViewModel)?.UpdateClient();
    }
    private async Task Login()
    {
        if (!string.IsNullOrEmpty(Client.Email) || !string.IsNullOrEmpty(Client.PhoneNumber))
        {
            Authorization.Login = IsCheckedEmail ? Client.Email : Client.PhoneNumber?.Replace(" ", "");
            Authorization.Password = Client.Password;
            
            var token = await _authorizationRepository.LoginClient(Authorization);
            await _refreshTokenRepository.AddRefreshToken(token.RefreshToken);
            _accessTokenRepository.AddAccessToken(token.AccessToken);
            
            var mainViewModel = Owner.Owner as MainViewModel ?? Owner.Owner.Owner as MainViewModel;
            mainViewModel.SelectedViewModel = Owner;
            (mainViewModel.SelectedViewModel as ProductsViewModel)?.UpdateClient();
        }
        else
            _notificationService.ShowNotification(new Notification(
                "Осторожно", "Заполните поля формы", NotificationType.Warning));
    }
    private async Task GoBack()
    {
        if (InsertLogin)
        {
            TextView = "Войдите в профиль";
            InsertLogin = false;
        }
        else
        {
            var mainViewModel = Owner.Owner as MainViewModel ?? Owner.Owner.Owner as MainViewModel;
            mainViewModel.SelectedViewModel = Owner;
        }
    }
    private void LoginThrownExc(Exception exp)
    {
        switch(exp)
        {
            case HttpRequestException:
                _notificationService.ShowNotification(new Notification("Ошибка", "Не удалось установить соединение с сервером", NotificationType.Error));
            break;
            case ValidationApiException:
                if(((ValidationApiException)exp).StatusCode == HttpStatusCode.Unauthorized)
                    _notificationService.ShowNotification(new Notification("Ошибка", "Неверный логин или пароль", NotificationType.Error));
                break;
        }
    }
}