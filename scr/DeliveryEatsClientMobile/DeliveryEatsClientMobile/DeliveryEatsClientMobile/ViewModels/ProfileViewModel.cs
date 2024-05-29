using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using DeliveryEatsClientMobile.Models;
using DeliveryEatsClientMobile.Service;
using desktop.Models;
using ReactiveUI;

namespace DeliveryEatsClientMobile.ViewModels;

public class ProfileViewModel : ViewModelBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IAccessTokenRepository _accessTokenRepository;
    private readonly IAuthorizationRepository _authorizationRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IUpdateTokenService _updateTokenService;
    private MainViewModel _mainViewModel;
    
    private List<MenuItem> _menu;
    private ClientDetails _clientDetails;
    public List<MenuItem> Menu
    {
        get => _menu;
        set => this.RaiseAndSetIfChanged(ref _menu, value);
    }
    public ClientDetails Client
    {
        get => _clientDetails;
        set => this.RaiseAndSetIfChanged(ref _clientDetails, value);
    }
    
    public ReactiveCommand<Unit, Unit> GetClientInfoCommand { get; }
    public ReactiveCommand<string, Unit> GoToCommand { get; }
    public ProfileViewModel(INotificationService notificationService, IAccessTokenRepository accessTokenRepository,
        IUpdateTokenService updateTokenService, IRefreshTokenRepository refreshTokenRepository, IAuthorizationRepository authorizationRepository, IOrderRepository orderRepository,
        IClientRepository clientRepository, Order order) : base(notificationService, updateTokenService, order)
    {
        _orderRepository = orderRepository;
        _accessTokenRepository = accessTokenRepository;
        _authorizationRepository = authorizationRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _clientRepository = clientRepository;
        _updateTokenService = updateTokenService;

        // Получение данных клиента.
        GetClientInfoCommand = ReactiveCommand.CreateFromTask(GetClientInfo);
        GetClientInfoCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GetClientInfoCommand));
        GetClientInfoCommand.Execute().Subscribe();
        // Переход по menuItems.
        GoToCommand = ReactiveCommand.CreateFromTask<string, Unit>(GoTo);
        GoToCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GoToCommand));
    }
    
    private async Task GetClientInfo()
    {
        var client = await _clientRepository.GetInfo(_accessTokenRepository.GetAccessToken());
        Client = await _clientRepository.GetClientDetails(_accessTokenRepository.GetAccessToken(), client.Id);
        var menuItems = new List<MenuItem>();
        menuItems.Add(new MenuItem() { Name = "Заказы", Icon = "FormatListBulleted" });
        menuItems.Add(new MenuItem() { Name = "Профиль", Icon = "Person" });
        menuItems.Add(new MenuItem() { Name = "Адреса", Icon = "LocationOn" });
        Menu = menuItems;
    }
    
    private async Task<Unit> GoTo(string nameMenuItem)
    {
        switch (nameMenuItem)
        {
            case "Заказы": break;
            case "Профиль": break;
            case "Адреса": break;
        }
        return Unit.Default;
    }
    
    public async Task Exit()
    {
        await _authorizationRepository.LogoutClient(_accessTokenRepository.GetAccessToken());
        _accessTokenRepository.DeleteAccessToken();
        await _refreshTokenRepository.DeleteRefreshToken();
        GoBack();
    }
    
    public async Task GoBack()
    {
        _mainViewModel = Owner.Owner as MainViewModel;
        _mainViewModel.SelectedViewModel = Owner;
        (_mainViewModel.SelectedViewModel as ProductsViewModel)?.GetClientInfoCommand.Execute();
    }
}