using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using DeliveryEatsClientMobile.Models;
using DeliveryEatsClientMobile.Service;
using DeliveryEatsClientMobile.Views;
using desktop.Models;
using ReactiveUI;

namespace DeliveryEatsClientMobile.ViewModels;

public class SearchViewModel : ViewModelBase
{
    private readonly IProductRepository _productRepository;
    // Продукты.
    private readonly ObservableAsPropertyHelper<IEnumerable<Product>> _products;
    private readonly ObservableAsPropertyHelper<ProductDetails> _productDetails;
    private readonly ObservableAsPropertyHelper<int> _countProducts;
    public IEnumerable<Product> Products => _products.Value;
    public ProductDetails ProductDetails => _productDetails.Value;
    public int CountProducts => _countProducts.Value;
    // Pagination.
    public OwnerParameters OwnerParameters { get; set; }
    // Bottom Sheet Classes.
    private bool? _bottomSheetClass = null;
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
    
    public ReactiveCommand<Unit, ProductCollection> GetProductsCommand { get; }
    public ReactiveCommand<int, ProductDetails> GetProductDetailsCommand { get; }
    public ReactiveCommand<int, Unit> AddProductCommand { get; }
    public ReactiveCommand<int, Unit> RemoveProductCommand { get; }
    public ReactiveCommand<ProductDetails, Unit> AddProductDetailsCommand { get; }
    public ReactiveCommand<ProductDetails, Unit> RemoveProductDetailsCommand { get; }
    
    public SearchViewModel(IProductRepository productRepository, INotificationService notificationService, 
        IAccessTokenRepository accessTokenRepository, IUpdateTokenService updateTokenService, 
        Order order) : base(notificationService, updateTokenService, order)
    {
        _productRepository = productRepository;

        // Получение списка продуктов
        GetProductsCommand = ReactiveCommand.CreateFromTask(GetProducts);
        GetProductsCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GetProductsCommand));
        _products = GetProductsCommand.Where(a => a != null)
            .Select(a => a.Product)
            .ToProperty(this, a => a.Products);
        // Получить подробную информацию по продукту.
        GetProductDetailsCommand = ReactiveCommand.CreateFromTask<int, ProductDetails>(GetProductDetails);
        GetProductDetailsCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GetProductDetailsCommand));
        _productDetails = GetProductDetailsCommand.ToProperty(this, a => a.ProductDetails);
        // Pagination.
        OwnerParameters = new OwnerParameters();
        _countProducts = GetProductsCommand.Where(a => a != null).Select(a => a.Count)
            .ToProperty(this, a => a.CountProducts);
        // Search.
        OwnerParameters.WhenAnyValue(a => a.SearchString).Where(a => a != null)
            .Throttle(TimeSpan.FromSeconds(0.75)).Subscribe(_ => GetProductsCommand.Execute().Subscribe());
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
    }
    
    private async Task<ProductCollection> GetProducts()
    {
        var response = await _productRepository.GetProducts(OwnerParameters);
        if (response == null)
            return null;
        // Создание словаря из существующих продуктов в заказе
        var orderProductCounts = _order.Products.ToDictionary(p => p.Id, p => p.Count);
        foreach (var product in response.Product)
            if (orderProductCounts.TryGetValue(product.Id, out var count))
                product.Count = count;
        return response;
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
        var product = Products.FirstOrDefault(a => a.Id == productId);
        var existingItem = _order.Products.FirstOrDefault(a => a.Id == productId);
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
        var product = Products.FirstOrDefault(a => a.Id == productId);
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
    public async Task GoBack()
    {
        var mainViewModel = Owner.Owner as MainViewModel ?? 
                            Owner.Owner?.Owner as MainViewModel ?? 
                            Owner.Owner?.Owner.Owner as MainViewModel;
        mainViewModel.SelectedViewModel = Owner;
        switch (mainViewModel.SelectedViewModel)
        {
            case ProductsViewModel productsViewModel:
            case FilterViewModel filterViewModel: 
                (mainViewModel.SelectedViewModel as dynamic).UpdateProduct(); 
                break;
        }
    }
    public async Task ShowBottomSheet() => BottomSheetClass = true;
    public async Task HideBottomSheet() => BottomSheetClass = false;
}