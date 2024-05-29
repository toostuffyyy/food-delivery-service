using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using DeliveryEatsClientMobile.Models;
using DeliveryEatsClientMobile.Service;
using desktop.Models;
using ReactiveUI;

namespace DeliveryEatsClientMobile.ViewModels;

public class ProductsViewModel : ViewModelBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryProductRepository _categoryProductRepository;
    private readonly IAccessTokenRepository _accessTokenRepository;
    private readonly IAuthorizationRepository _authorizationRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IUpdateTokenService _updateTokenService;
    private readonly Order _order;
    // Продукты.
    private readonly ObservableAsPropertyHelper<List<GroupProduct>> _products;
    private readonly ObservableAsPropertyHelper<ProductDetails> _productDetails;
    private Client _client;
    public List<GroupProduct> Products => _products.Value;
    public ProductDetails ProductDetails => _productDetails.Value;
    public Client Client
    {
        get => _client;
        set => this.RaiseAndSetIfChanged(ref _client, value);
    }
    // Категория продуктов.
    private readonly ObservableAsPropertyHelper<List<CategoryProduct>> _categoryProduct;
    public List<CategoryProduct> CategoryProducts => _categoryProduct.Value;
    // Pagination.
    public OwnerParameters OwnerParameters { get; set; }
    // Bottom Sheet Classes.
    private bool? _bottomSheetClass;
    public bool? BottomSheetClass
    {
        get => _bottomSheetClass;
        set => this.RaiseAndSetIfChanged(ref _bottomSheetClass, value);
    }
    // Bottom Sheet Margin.
    private Thickness _marginBottomSheet;
    public Thickness MarginBottomSheet
    {
        get => _marginBottomSheet;
        set => this.RaiseAndSetIfChanged(ref _marginBottomSheet, value);
    }
    
    public ReactiveCommand<Unit, Unit> GetClientInfoCommand { get; }
    public ReactiveCommand<Unit, List<GroupProduct>> GetProductsCommand { get; }
    public ReactiveCommand<Unit, List<CategoryProduct>> GetCategoryProductsCommand { get; }
    public ReactiveCommand<int, ProductDetails> GetProductDetailsCommand { get; }
    public ReactiveCommand<int, Unit> AddProductCommand { get; }
    public ReactiveCommand<int, Unit> RemoveProductCommand { get; }
    public ReactiveCommand<ProductDetails, Unit> AddProductDetailsCommand { get; }
    public ReactiveCommand<ProductDetails, Unit> RemoveProductDetailsCommand { get; }
    public ReactiveCommand<int, Unit> GoToFilterCommand { get; }
    
    public ProductsViewModel(IProductRepository productRepository, IOrderRepository orderRepository, INotificationService notificationService,
        IAccessTokenRepository accessTokenRepository, IRefreshTokenRepository refreshTokenRepository, IAuthorizationRepository authorizationRepository,
        ICategoryProductRepository categoryProductRepository, IClientRepository clientRepository,
        IUpdateTokenService updateTokenService, Order order) : base(notificationService, updateTokenService, order)
    {
        _productRepository = productRepository;
        _categoryProductRepository = categoryProductRepository;
        _orderRepository = orderRepository;
        _accessTokenRepository = accessTokenRepository;
        _authorizationRepository = authorizationRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _clientRepository = clientRepository;
        _order = order;

        // Получение данных клиента.
        GetClientInfoCommand = ReactiveCommand.CreateFromTask(GetClientInfo);
        GetClientInfoCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GetClientInfoCommand));
        GetClientInfoCommand.Execute().Subscribe();
        // Получение списка продуктов
        GetProductsCommand = ReactiveCommand.CreateFromTask(GetProducts);
        GetProductsCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GetProductsCommand));
        _products = GetProductsCommand.Where(a => a != null)
            .ToProperty(this, a => a.Products, scheduler: RxApp.MainThreadScheduler);
        GetProductsCommand.Execute().Subscribe();
        // Получить подробную информацию по продукту.
        GetProductDetailsCommand = ReactiveCommand.CreateFromTask<int, ProductDetails>(GetProductDetails);
        GetProductDetailsCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GetProductDetailsCommand));
        _productDetails = GetProductDetailsCommand.ToProperty(this, a => a.ProductDetails);
        // Получение списка категории продукта.
        GetCategoryProductsCommand = ReactiveCommand.CreateFromTask(GetCategoryProducts);
        GetCategoryProductsCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GetCategoryProductsCommand));
        _categoryProduct = GetCategoryProductsCommand.ToProperty(this, a => a.CategoryProducts);
        GetCategoryProductsCommand.Execute().Subscribe();
        // Pagination.
        OwnerParameters = new OwnerParameters();
        // Добавление продукта.
        AddProductCommand = ReactiveCommand.CreateFromTask<int>(AddProduct);
        AddProductCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, AddProductCommand));
        // Удаление продукта.
        RemoveProductCommand = ReactiveCommand.CreateFromTask<int>(RemoveProduct);
        RemoveProductCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, RemoveProductCommand)); 
        // Добавление продукта в подробном представлении.
        AddProductDetailsCommand = ReactiveCommand.CreateFromTask<ProductDetails>(AddProductDetails);
        AddProductDetailsCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, AddProductDetailsCommand));
        // Удаление продукта в подробном представлении.
        RemoveProductDetailsCommand = ReactiveCommand.CreateFromTask<ProductDetails>(RemoveProductDetails);
        RemoveProductDetailsCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, RemoveProductDetailsCommand)); 
        // Переход к фильтрации.
        GoToFilterCommand = ReactiveCommand.CreateFromTask<int>(GoToFilter);
        GoToFilterCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GoToFilterCommand));
    }
    
    private async Task<List<GroupProduct>> GetProducts()
    {
        var response = await _productRepository.GetProducts(OwnerParameters);
        var allCategories = await _categoryProductRepository.GetCategoryProduct();
        
        if (response == null || allCategories == null)
            return null;
        
        var groupedProducts = response.Product
            .Where(p => p != null && p.AvailableQuantity > 0)
            .GroupBy(p =>
            {
                var category = allCategories.FirstOrDefault(cp => cp.Id == (p.CategoryProduct.ParentCategoryProductId ?? p.CategoryProduct.Id));
                while (category?.ParentCategoryProductId != null)
                    category = allCategories.FirstOrDefault(cp => cp.Id == category.ParentCategoryProductId);
                return new
                { 
                    Id = category?.Id ?? -1,
                    Name = category?.Name ?? "Другое" 
                };
            })
            .Select(group => new GroupProduct
            {
                Id = group.Key.Id,
                Name = group.Key.Name,
                Products = group.ToList()
            }).ToList();
        return groupedProducts;
    }
    private async Task<ProductDetails> GetProductDetails(int productId)
    {
        ShowBottomSheet();
        var response = await _productRepository.GetProductDetails(productId);
        if (response == null)
            return null;
        var existingItem = _order.Products.FirstOrDefault(a => a.Id == productId);
        if (existingItem != null)
            response.Count = existingItem.Count;
        return response;
    }
    private async Task AddProduct(int productId)
    {
        var existingItem = _order.Products.FirstOrDefault(a => a.Id == productId);
        var product = Products.SelectMany(a => a.Products)
            .FirstOrDefault(b => b.Id == productId);
        if (existingItem != null)
        {
            existingItem.Count++;
            product.Count = existingItem.Count;
        }
        else
        {
            product.Count = 1;
            _order.Products.Add(product);
        }
    }
    private async Task RemoveProduct(int productId)
    {
        var existingItem = _order.Products.FirstOrDefault(a => a.Id == productId);
        var product = Products.SelectMany(a => a.Products)
            .FirstOrDefault(b => b.Id == productId);
        if (existingItem != null)
        {
            if (existingItem.Count == 1)
            {
                existingItem.Count--;
                _order.Products.Remove(existingItem);
            }
            else
                existingItem.Count--;
            product.Count = existingItem?.Count ?? 0;
        }
    }
    private async Task AddProductDetails(ProductDetails productDetails)
    {
        AddProduct(productDetails.Id);
        productDetails.Count++;
    }
    private async Task RemoveProductDetails(ProductDetails productDetails)
    {
        RemoveProduct(productDetails.Id);
        productDetails.Count--;
    }
    public void UpdateProduct()
    {
        // Создание словаря из существующих продуктов в заказе
        var orderProductCounts = _order.Products.ToDictionary(p => p.Id, p => p.Count);
        foreach (var item in _products.Value.SelectMany(product => product.Products))
            if (orderProductCounts.TryGetValue(item.Id, out var count))
                item.Count = count;
            else
                item.Count = 0;
    }
    private async Task GetClientInfo()
    {
        var token = await _refreshTokenRepository.GetRefreshToken();

        if (token == null)
        {
            Client = null;
            return;
        }
        
        var newTokens = await _authorizationRepository.UpdateTokenClient(token);
        _refreshTokenRepository.UpdateRefreshToken(newTokens.RefreshToken);
        _accessTokenRepository.AddAccessToken(newTokens.AccessToken);

        UpdateClient();
    }
    public async void UpdateClient()
    {
        Client = await _clientRepository.GetInfo(_accessTokenRepository.GetAccessToken());
        Client.SelectedAddress = Client.Addresses.FirstOrDefault();
    }
    private async Task<List<CategoryProduct>> GetCategoryProducts()
    {
        var response = await _categoryProductRepository.GetCategoryProductMainPage();
        return response != null ? response : null;
    }
    public void GoToSearch()
    {
        (Owner as MainViewModel).SelectedViewModel = new SearchViewModel(_productRepository, 
            _notificationService, _accessTokenRepository, _updateTokenService, _order) { Owner = this };
    }
    public async Task GoToFilter(int categoryProductId)
    {
        if (categoryProductId == 0)
            GoToAllFilter();
        else
            (Owner as MainViewModel).SelectedViewModel = new FilterViewModel(categoryProductId, _productRepository,
                    _notificationService, _accessTokenRepository, _categoryProductRepository,
                    _updateTokenService, _order) { Owner = this };
    }
    public async Task GoToProfile()
    {
        (Owner as MainViewModel).SelectedViewModel = new ProfileViewModel(_notificationService, _accessTokenRepository, 
            _updateTokenService, _refreshTokenRepository, _authorizationRepository, _orderRepository, _clientRepository,
            _order) { Owner = this };
    }
    public async Task GoToAllFilter()
    {
        (Owner as MainViewModel).SelectedViewModel = new AllFilterViewModel(_productRepository,
            _notificationService, _accessTokenRepository, _categoryProductRepository,
            _updateTokenService, _order) { Owner = this };
    }
    public void GoToAuthorization()
    {
        (Owner as MainViewModel).SelectedViewModel = new AuthorizationViewModel(_accessTokenRepository, 
            _refreshTokenRepository, _authorizationRepository, _notificationService,
            _updateTokenService, _clientRepository, _order) { Owner = this };
    }
    public void GoToCart()
    {
        (Owner as MainViewModel).SelectedViewModel = new CartViewModel(_orderRepository,
            _notificationService, _accessTokenRepository, _updateTokenService, _clientRepository, _order) { Owner = this };
    }
    public async Task ShowBottomSheet() => BottomSheetClass = true;
    public async Task HideBottomSheet() => BottomSheetClass = false;
}