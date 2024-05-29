using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using DeliveryEatsClientMobile.Models;
using DeliveryEatsClientMobile.Service;
using ReactiveUI;

namespace DeliveryEatsClientMobile.ViewModels;

public class AllFilterViewModel : ViewModelBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryProductRepository _categoryProductRepository;
    private readonly IAccessTokenRepository _accessTokenRepository;
    private readonly IUpdateTokenService _updateTokenService;
    private Order _order;
    // Категория продуктов.
    private readonly ObservableAsPropertyHelper<List<CategoryProduct>> _categoryProduct;
    public List<CategoryProduct> CategoryProducts => _categoryProduct.Value;
    
    public ReactiveCommand<Unit, List<CategoryProduct>> GetCategoryProductsCommand { get; }
    public ReactiveCommand<int, Unit> GoToFilterCommand { get; }

    public AllFilterViewModel(IProductRepository productRepository, INotificationService notificationService, 
        IAccessTokenRepository accessTokenRepository, ICategoryProductRepository categoryProductRepository, 
        IUpdateTokenService updateTokenService, Order order) : base(notificationService, updateTokenService, order)
    {
        _productRepository = productRepository;
        _categoryProductRepository = categoryProductRepository;
        _accessTokenRepository = accessTokenRepository;
        _order = order;
        
        // Получение списка категории продукта.
        GetCategoryProductsCommand = ReactiveCommand.CreateFromTask(GetCategoryProducts);
        GetCategoryProductsCommand.Execute().Subscribe();
        GetCategoryProductsCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GetCategoryProductsCommand));
        _categoryProduct = GetCategoryProductsCommand.ToProperty(this, a => a.CategoryProducts);
        // Переход к фильтрации.
        GoToFilterCommand = ReactiveCommand.CreateFromTask<int>(GoToFilter);
        GoToFilterCommand.ThrownExceptions.Subscribe(async a => await CommandExc(a, GoToFilterCommand));
    }
    
    private async Task GoToFilter(int categoryProductId)
    {
        (Owner.Owner as MainViewModel).SelectedViewModel = new FilterViewModel(categoryProductId, _productRepository,
            _notificationService, _accessTokenRepository, _categoryProductRepository,
            _updateTokenService, _order) { Owner = this };
    }
    private async Task<List<CategoryProduct>> GetCategoryProducts()
    {
        var response = await _categoryProductRepository.GetCategoryProduct();
        response = response.Where(a => a.ParentCategoryProductId == null).ToList();
        return response != null ? response : null;
    }
    public async Task GoBack()
    {
        var mainViewModel = Owner.Owner as MainViewModel;
        mainViewModel.SelectedViewModel = Owner;
        (mainViewModel.SelectedViewModel as ProductsViewModel)?.UpdateProduct();
    }
}