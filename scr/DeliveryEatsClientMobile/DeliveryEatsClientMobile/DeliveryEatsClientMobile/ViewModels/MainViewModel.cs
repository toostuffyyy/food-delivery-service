using DeliveryEatsClientMobile.Models;
using DeliveryEatsClientMobile.Service;
using ReactiveUI;

namespace DeliveryEatsClientMobile.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ViewModelBase _selectedViewModel;

    public ViewModelBase SelectedViewModel
    {
        get => _selectedViewModel;
        set => this.RaiseAndSetIfChanged(ref _selectedViewModel, value);
    }

    public MainViewModel(IProductRepository productRepository, IOrderRepository orderRepository, INotificationService notificationService, 
        IAccessTokenRepository accessTokenRepository, IRefreshTokenRepository refreshTokenRepository, IAuthorizationRepository authorizationRepository,
        ICategoryProductRepository categoryProductRepository, IClientRepository clientRepository,
        IUpdateTokenService updateTokenService, Order order): base(notificationService, updateTokenService, order)
    {
        order = new() { Products = [] };
        SelectedViewModel = new ProductsViewModel(productRepository, orderRepository, notificationService, 
            accessTokenRepository, refreshTokenRepository, authorizationRepository, categoryProductRepository,
            clientRepository, updateTokenService, order) { Owner = this };
    }
}