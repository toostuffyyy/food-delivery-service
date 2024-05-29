using desktop.Models;
using desktop.Service;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using desktop.Tools;
using DynamicData;
using DynamicData.Binding;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace desktop.ViewModels;

public class CatalogOrdersViewModel : ViewModelBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IAccessTokenRepository _accessTokenRepository;
    private readonly ObservableAsPropertyHelper<bool> _isLoadingOrders;
    private readonly IDialogService _dialogService;
    private readonly ObservableAsPropertyHelper<IEnumerable<Order>> _orders;
    // Filter.
    private readonly ICategoryProductRepository _categoryProductRepository;
    private readonly IStatusRepository _statusRepository;
    private readonly IStatusPayRepository _statusPayRepository;
    
    private ReactiveCommand<Unit, Unit> _cancelCommand;
    private Order _selectedOrder;
    public Order SelectedOrder
    {
        get => _selectedOrder;
        set => this.RaiseAndSetIfChanged(ref _selectedOrder, value);
    }
    // Pagination.
    private readonly ObservableAsPropertyHelper<int> _countPage;
    private readonly ObservableAsPropertyHelper<int> _countOrders;
    public int CountPage => _countPage.Value;
    public int CountOrders => _countOrders.Value;
    public IEnumerable<int> PageCounts => new[] { 5, 9, 15, 20, 25 };
    public OwnerParameters OwnerParameters { get; set; }
    public List<FilterCategory> Filters { get; set; }
    // Sorting.
    public ObservableCollection<SortElement> SortElements { get; private set; }
    // OrderDetails.
    private readonly ObservableAsPropertyHelper<bool> _isLoadingOrderDetails;
    private readonly ObservableAsPropertyHelper<OrderDetails> _orderDetails;
    public bool IsLoadingOrderDetails => _isLoadingOrderDetails.Value;
    public OrderDetails SelectedOrderDetails => _orderDetails.Value;
    public ReactiveCommand<Order, OrderDetails> LoadingOrderDetails { get; set; }

    private async Task<OrderDetails> GetSelectedOrderDetails(Order selectedOrder) => 
        await _orderRepository.GetOrder(_accessTokenRepository.GetAccessToken(), selectedOrder.Id);
    
    public bool IsLoadingOrders => _isLoadingOrders.Value;
    public IEnumerable<Order> Orders => _orders.Value;
    public ReactiveCommand<Unit, OrdersCollection> LoadingOrders { get; private set; }
    // Delete Order.
    public ReactiveCommand<int, Unit> DeleteOrderCommand { set; get; }
    // Update Order.
    private bool _isVisibleOrderButton;
    private string _contentOrderButton;

    public bool IsVisibleOrderButton
    {
        get => _isVisibleOrderButton;
        set => this.RaiseAndSetIfChanged(ref _isVisibleOrderButton, value);
    }
    public string ContentOrderButton
    {
        get => _contentOrderButton;
        set => this.RaiseAndSetIfChanged(ref _contentOrderButton, value);
    }

    public CatalogOrdersViewModel(IOrderRepository orderRepository, 
                                  IAccessTokenRepository accessTokenRepository,
                                  INotificationService notificationService,
                                  IUpdateTokenService updateTokenService,
                                  ICategoryProductRepository categoryProductRepository,
                                  IStatusRepository statusRepository,
                                  IStatusPayRepository statusPayRepository,
                                  IDialogService dialogService) : base(notificationService, updateTokenService)
    {
        _orderRepository = orderRepository;
        _accessTokenRepository = accessTokenRepository;
        _categoryProductRepository = categoryProductRepository;
        _statusRepository = statusRepository;
        _statusPayRepository = statusPayRepository;
        _statusPayRepository = statusPayRepository;
        _dialogService = dialogService;
        
        // GetOrders.
        LoadingOrders = ReactiveCommand
            .CreateFromObservable(() => Observable
                .StartAsync(ct => LoadingOrdersTask(ct))
                .TakeUntil(_cancelCommand));
        LoadingOrders.IsExecuting.ToProperty(this, a => a.IsLoadingOrders, out _isLoadingOrders);
        LoadingOrders.ThrownExceptions.Subscribe(async a => await CommandExc(a, LoadingOrders));
        _orders = LoadingOrders.Where(a => a != null).Select(x => x.Orders)
            .ToProperty(this, a => a.Orders, scheduler: RxApp.MainThreadScheduler);
        _cancelCommand = ReactiveCommand.Create(() => { }, LoadingOrders.IsExecuting);
        // GetOrderDetails.
        LoadingOrderDetails = ReactiveCommand.CreateFromTask<Order, OrderDetails>(GetSelectedOrderDetails);
        LoadingOrderDetails.IsExecuting.ToProperty(this, a => a.IsLoadingOrderDetails, out _isLoadingOrderDetails);
        LoadingOrderDetails.ThrownExceptions.Subscribe(async x => await CommandExc(x, LoadingOrders));
        this.WhenAnyValue(a => a.SelectedOrder).Where(a => a != null).InvokeCommand(LoadingOrderDetails);
        _orderDetails = LoadingOrderDetails.ToProperty(this, a => a.SelectedOrderDetails);
        this.WhenAnyValue(x => x.SelectedOrderDetails.Status)
            .Subscribe(_ =>
            {
                switch (SelectedOrderDetails.Status)
                {
                    case "В обработке": IsVisibleOrderButton = true; ContentOrderButton = "Принять заказ"; break;
                    case "Ожидает": IsVisibleOrderButton = true; ContentOrderButton = "Сборка заказа"; break;
                    case "Сборка": IsVisibleOrderButton = true; ContentOrderButton = "В пути"; break;
                    default: IsVisibleOrderButton = false; break;
                }
            });
        // DeleteOrder.
        DeleteOrderCommand = ReactiveCommand.CreateFromTask<int>(DeleteOrder);
        DeleteOrderCommand.ThrownExceptions.Subscribe(async exp => await CommandExc(exp, LoadingOrders));
        // Pagination.
        OwnerParameters = new OwnerParameters();
        _countOrders = LoadingOrders.Where(x => x != null).Select(a => a.Count)
            .ToProperty(this, x => x.CountOrders, scheduler: RxApp.MainThreadScheduler);
        this.WhenAnyValue(a => a.CountOrders, a => a.OwnerParameters.SizePage)
            .Where(x => x.Item2 != 0)
            .Select(x => (x.Item1 + x.Item2 - 1) / x.Item2)
            .ToProperty(this, x => x.CountPage, out _countPage);
        OwnerParameters.WhenAnyValue(a => a.PageNumber).Subscribe(_ => RestartLoadOrders());
        OwnerParameters.WhenAnyValue(a => a.SizePage).Subscribe(_ => GoFirstAndRestartLoadOrders());
        // Search.
        OwnerParameters.WhenAnyValue(a => a.SearchString).Where(a => a != null)
            .Throttle(TimeSpan.FromSeconds(0.5)).Subscribe(_ => GoFirstAndRestartLoadOrders());
        // Filter.
        Filters = new List<FilterCategory>()
        {
            new() { NameCategory = "Категория продуктов", ParameterName = "categoryproduct" },
            new() { NameCategory = "Статус", ParameterName = "status" },
            new() { NameCategory = "Статус оплаты", ParameterName = "statuspay" }
        };
        OwnerParameters.WhenAnyValue(a => a.Filters).Subscribe(_ => GoFirstAndRestartLoadOrders());
        // Sorting.
        SortElements = new ObservableCollection<SortElement>()
        {
            new() { Sorting = new() { NameColumn = "numberorder" }, NameSort = "Номер заказа" },
            new() { Sorting = new() { NameColumn = "startdate" }, NameSort = "Дата" }
        };
        SortElements.ToObservableChangeSet()
            .AutoRefresh(a => a.IsSelected)
            .ToCollection()
            .Subscribe(a =>
            {
                OwnerParameters.Sorts = JsonSerializer.Serialize(a.Where(x => x.IsSelected).Select(p => p.Sorting).ToList());
            });
        SortElements.ToObservableChangeSet()
            .AutoRefresh(a => a.Sorting.Direction)
            .ToCollection()
            .Subscribe(a =>
            {
                OwnerParameters.Sorts = JsonSerializer.Serialize(a.Where(x => x.IsSelected).Select(p => p.Sorting).ToList());
            });
        OwnerParameters.WhenAnyValue(a => a.Sorts).Subscribe(_ => GoFirstAndRestartLoadOrders());
        ReactiveCommand.CreateFromTask(LoadFilters).Execute();
    }

    public async void CancelStatusOrder()
    {
        OrderStatus? orderStatus = new OrderStatus()
        {
            Id = SelectedOrderDetails.Id,
            StatusId = 6
        };
        var result = await _dialogService.ShowDialog("Подтвеждение статуса", "Вы действительно хотите отменить заказ?", IDialogService.DialogType.YesNoDialog);
        if (result == IDialogService.DialogResult.Yes)
        {
            await _orderRepository.UpdateStatusOrder(_accessTokenRepository.GetAccessToken(), orderStatus);
            RestartLoadOrders(); 
        }
    }
    
    public async void UpdateStatusOrder()
    {
        OrderStatus? orderStatus = new OrderStatus()
        {
            Id = SelectedOrderDetails.Id,
            StatusId = SelectedOrderDetails.StatusId + 1
        };
        var result = await _dialogService.ShowDialog("Подтвеждение статуса", "Вы действительно хотите изменить статус " +
            $"{SelectedOrderDetails.Status.ToLower()} на следующий?", IDialogService.DialogType.YesNoDialog);
        if (result == IDialogService.DialogResult.Yes)
        {
            await _orderRepository.UpdateStatusOrder(_accessTokenRepository.GetAccessToken(), orderStatus);
            RestartLoadOrders();
        }
    }
    
    private async Task<OrdersCollection> LoadingOrdersTask(CancellationToken ct)
    {
        var accessToken = _accessTokenRepository.GetAccessToken();
        if (ct.IsCancellationRequested)
            return null;
        var ordersCollection = await _orderRepository.GetOrders(accessToken, OwnerParameters);
        if (ct.IsCancellationRequested)
            return null;
        SelectedOrder = ordersCollection.Orders.Count() > 0 ? ordersCollection.Orders.ToList()[0] : null;
        return ordersCollection;
    }

    private List<FilterParameter> GetFilterParameters()
    {
        List<FilterParameter> filtersParameters = new List<FilterParameter>();
        foreach (FilterCategory filterCategory in Filters)
        {
            if (filterCategory.Filters == null)
                break;
            foreach (Filter filter in filterCategory.Filters.Where(a => a.IsPick))
            {
                filtersParameters.Add(new FilterParameter()
                    { NameParameter = filterCategory.ParameterName, Value = filter.Value});
            }
        }
        return filtersParameters;
    }

    private async Task LoadFilters()
    {
        var categoryProduct = await _categoryProductRepository.GetCategoryProduct();
        Filters[0].Filters =
            new ObservableCollection<Filter>(categoryProduct
                .Where(a => a.ParentCategoryProductId != null)
                .Select(a => new Filter()
                { NameFilter = a.Name, Value = a.Id }));
        
        var status = await _statusRepository.GetStatus();
        Filters[1].Filters =
            new ObservableCollection<Filter>(status.Select(a => new Filter()
                { NameFilter = a.Name, Value = a.Id }));
        
        var statusPay = await _statusPayRepository.GetStatusPay();
        Filters[2].Filters =
            new ObservableCollection<Filter>(statusPay.Select(a => new Filter()
                { NameFilter = a.Name, Value = a.Id }));
        
        Filters.ForEach(x => x.Filters.ToObservableChangeSet()
            .AutoRefresh(r => r.IsPick)
            .Subscribe(_ =>
            {
                var filterParameters = GetFilterParameters();
                OwnerParameters.Filters = JsonSerializer.Serialize(filterParameters);
            }));
    }

    private async Task DeleteOrder(int id)
    {
        var result = await _dialogService.ShowDialog("Удаление заказа", "Вы действительно хотите удалить заказ?",
            IDialogService.DialogType.YesNoDialog);
        if (result == IDialogService.DialogResult.Yes)
        {
            await _orderRepository.DeleteOrder(_accessTokenRepository.GetAccessToken(), id);
            GoFirstAndRestartLoadOrders();
        }
    }
    
    public void NextPage()
    {
        if (OwnerParameters.PageNumber < CountPage)
            OwnerParameters.PageNumber++;
    }
    public void PreviousPage()
    {
        if (OwnerParameters.PageNumber > 1)
            OwnerParameters.PageNumber--;
    }
    public void GoFirstPage() => OwnerParameters.PageNumber = 1;
    public void GoLastPage() => OwnerParameters.PageNumber = CountPage;

    private void RestartLoadOrders()
    {
        _cancelCommand.Execute().Subscribe();
        LoadingOrders.Execute().Subscribe();
    }

    private void GoFirstAndRestartLoadOrders()
    {
        if (OwnerParameters.PageNumber == 1)
            RestartLoadOrders();
        else OwnerParameters.PageNumber = 1;
    }
}