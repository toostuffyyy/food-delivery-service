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

public class FilterViewModel : ViewModelBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryProductRepository _categoryProductRepository;
    private readonly IAccessTokenRepository _accessTokenRepository;
    private readonly Order _order;
    // Продукты.
    private readonly ObservableAsPropertyHelper<List<GroupProduct>> _products;
    private readonly ObservableAsPropertyHelper<ProductDetails> _productDetails;
    private readonly ObservableAsPropertyHelper<CategoryProduct> _categoryProduct;
    public List<GroupProduct> Products => _products.Value;
    public ProductDetails ProductDetails => _productDetails.Value;
    public CategoryProduct CategoryProduct => _categoryProduct.Value;
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
    
    public ReactiveCommand<Unit, List<GroupProduct>> GetProductsCommand { get; }
    public ReactiveCommand<int, ProductDetails> GetProductDetailsCommand { get; }
    public ReactiveCommand<Unit, CategoryProduct> GetCategoryProductCommand { get; }
    public ReactiveCommand<int, Unit> AddProductCommand { get; }
    public ReactiveCommand<int, Unit> RemoveProductCommand { get; }
    public ReactiveCommand<ProductDetails, Unit> AddProductDetailsCommand { get; }
    public ReactiveCommand<ProductDetails, Unit> RemoveProductDetailsCommand { get; }
    public ReactiveCommand<Unit, Unit> GoBackCommand { get; }

    public FilterViewModel(int productCategoryId, IProductRepository productRepository, INotificationService notificationService, 
        IAccessTokenRepository accessTokenRepository, ICategoryProductRepository categoryProductRepository,
        IUpdateTokenService updateTokenService, Order order) : base(notificationService, updateTokenService, order)
    {
        _productRepository = productRepository;
        _categoryProductRepository = categoryProductRepository;
        _accessTokenRepository = accessTokenRepository;
        _order = order;

        // Pagination.
        OwnerParameters = new OwnerParameters() {Filters = productCategoryId.ToString()};
        // Получение списка продуктов
        GetProductsCommand = ReactiveCommand.CreateFromTask(GetProducts);
        GetProductsCommand.Execute().Subscribe();
        GetProductsCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GetProductsCommand));
        _products = GetProductsCommand.Where(a => a != null)
            .ToProperty(this, a => a.Products);
        // Получить подробную информацию по продукту.
        GetProductDetailsCommand = ReactiveCommand.CreateFromTask<int, ProductDetails>(GetProductDetails);
        GetProductDetailsCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GetProductDetailsCommand));
        _productDetails = GetProductDetailsCommand.ToProperty(this, a => a.ProductDetails);
        // Получить категорию по id.
        GetCategoryProductCommand = ReactiveCommand.CreateFromTask(GetCategoryProduct);
        GetCategoryProductCommand.Execute().Subscribe();
        GetCategoryProductCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GetCategoryProductCommand));
        _categoryProduct = GetCategoryProductCommand.ToProperty(this, a => a.CategoryProduct);
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
        // Переход к продуктам.
        GoBackCommand = ReactiveCommand.CreateFromTask(GoBack);
        GoBackCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GoBackCommand));
    }
    private async Task<List<GroupProduct>> GetProducts()
    {
        var response = await _productRepository.GetProducts(OwnerParameters);
    
        if (response == null || response.Product == null)
            return null;

        var orderProductCounts = _order.Products.ToDictionary(a => a.Id, a => a.Count);
        
        var groupedProducts = response.Product
            .Where(p => p != null && p.AvailableQuantity > 0)
            .GroupBy(p => p.CategoryProduct.Name)
            .Select(group => new GroupProduct
            {
                Name = group.Key,
                Products = group.Select(a =>
                {
                    if (orderProductCounts.TryGetValue(a.Id, out var count))
                        a.Count = count;
                    return a;
                }).ToList()
            })
            .ToList();
        return groupedProducts;
    }
    private async Task<ProductDetails> GetProductDetails(int productId)
    {
        ShowBottomSheet();
        var response = await _productRepository.GetProductDetails(productId);
        var existingItem = _order.Products.FirstOrDefault(a => a.Id == productId);
        if (existingItem != null)
            response.Count = existingItem.Count;
        return response != null ? response : null;
    }
    private async Task<CategoryProduct> GetCategoryProduct()
    {
        var response = await _categoryProductRepository
            .GetCategoryProduct(int.Parse(OwnerParameters.Filters));
        return response != null ? response : null;
    }
    private async Task AddProduct(int productId)
    {
        var product = Products.SelectMany(a => a.Products)
            .FirstOrDefault(a => a.Id == productId);
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
        var product = Products.SelectMany(a => a.Products)
            .FirstOrDefault(a => a.Id == productId);
        var existingItem = _order.Products.FirstOrDefault(a => a.Id == productId);
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
    private async Task GoBack()
    {
        var mainViewModel = Owner.Owner as MainViewModel ?? Owner.Owner?.Owner as MainViewModel;
        mainViewModel.SelectedViewModel = Owner;
        (mainViewModel.SelectedViewModel as ProductsViewModel)?.UpdateProduct();
    }
    public async Task GoSearch()
    {
        var mainViewModel = Owner.Owner as MainViewModel ?? Owner.Owner?.Owner as MainViewModel;
        mainViewModel.SelectedViewModel = new SearchViewModel(_productRepository, _notificationService, 
            _accessTokenRepository, _updateTokenService, _order) { Owner = this };
    }
    public async Task ShowBottomSheet() => BottomSheetClass = true;
    public async Task HideBottomSheet() => BottomSheetClass = false;
}