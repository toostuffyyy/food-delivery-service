using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using desktop.Models;
using desktop.Service;
using desktop.Services;
using ReactiveUI;
using MenuItem = desktop.Models.MenuItem;

namespace desktop.ViewModels;

public class EmployeeViewModel : ViewModelBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAuthorizationRepository _authorizationRepository;
    private readonly IAccessTokenRepository _accessTokenRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IViewNavigation _viewNavigation;
    private Lazy<CatalogOrdersViewModel> _catalogOrdersViewModel;
    private Lazy<CatalogEmployeesViewModel> _catalogEmployeesViewModel;
    private Lazy<ReportViewModel> _reportViewModel;
    private List<MenuItem> _menu;
    private MenuItem _selectedMenuItem;
    private ViewModelBase _selectedViewModel;
    private readonly ObservableAsPropertyHelper<Employee> _employee;
    private readonly ObservableAsPropertyHelper<bool> _isEmployeeInfoLoading;

    public bool IsEmployeeInfoLoading => _isEmployeeInfoLoading.Value;
    public Employee Employee => _employee.Value;
    public bool IsAdmin { get; private set; }
    public ReactiveCommand<Unit, Employee> GetEmployeeInfoCommand { get; }
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }

    public EmployeeViewModel(IEmployeeRepository employeeRepository,
        IAuthorizationRepository authorizationRepository,
        IAccessTokenRepository accessTokenRepository,
        INotificationService notificationService,
        IUpdateTokenService updateTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IViewNavigation viewNavigation,
        IOrderRepository orderReporitory,
        ICategoryProductRepository categoryProductRepository,
        IStatusRepository statusRepository,
        IStatusPayRepository statusPayRepository,
        IRoleRepository roleRepository,
        IDialogService dialogService,
        IStatisticRepository statisticRepository,
        IReportProductService productService) : base(notificationService, updateTokenService)
    {
        _employeeRepository = employeeRepository;
        _authorizationRepository = authorizationRepository;
        _accessTokenRepository = accessTokenRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _viewNavigation = viewNavigation;
        
        ExitCommand = ReactiveCommand.CreateFromTask(Exit);
        ExitCommand.ThrownExceptions.Subscribe(async x => CommandExc(x, ExitCommand));
        this.WhenAnyValue(a => a.SelectedMenuItem).Where(a => a != null).Subscribe(x =>
        {
            foreach (var item in Menu)
                item.IsSelected = item == x;
            
            if (x.Name == "Заказы")
                SelectedViewModel = _catalogOrdersViewModel.Value;
            else if (x.Name == "Персонал")
                SelectedViewModel = _catalogEmployeesViewModel.Value;
            else if (x.Name == "Статистика")
                SelectedViewModel = _reportViewModel.Value;
        });
        GetEmployeeInfoCommand = ReactiveCommand.CreateFromTask(GetEmployeeInfo);
        GetEmployeeInfoCommand.Execute().Subscribe();
        GetEmployeeInfoCommand.ThrownExceptions.Subscribe(async exc => CommandExc(exc, GetEmployeeInfoCommand));
        GetEmployeeInfoCommand.IsExecuting.ToProperty(this, a => a.IsEmployeeInfoLoading, out _isEmployeeInfoLoading);
        _employee = GetEmployeeInfoCommand.ToProperty(this, a => a.Employee);
        
        _catalogOrdersViewModel = new Lazy<CatalogOrdersViewModel>(() => new CatalogOrdersViewModel(orderReporitory,
            accessTokenRepository, _notificationService, updateTokenService, categoryProductRepository,statusRepository, statusPayRepository, dialogService));
        
        _catalogEmployeesViewModel = new Lazy<CatalogEmployeesViewModel>(() => new CatalogEmployeesViewModel(employeeRepository,
            accessTokenRepository, _notificationService, updateTokenService, dialogService, roleRepository, viewNavigation));

        _reportViewModel = new Lazy<ReportViewModel>(() => new ReportViewModel(accessTokenRepository,
            updateTokenService, notificationService, statisticRepository, productService));
    }
    public async Task Exit()
    {
        await _authorizationRepository.LogoutEmployee(_accessTokenRepository.GetAccessToken());
        _accessTokenRepository.DeleteAccessToken();
        await _refreshTokenRepository.DeleteRefreshToken();
        _viewNavigation.GoToAndCloseOthers<LoginViewModel>();
    }

    private async Task<Employee> GetEmployeeInfo()
    {
        Employee employee = await _employeeRepository.GetInfo(_accessTokenRepository.GetAccessToken());
        var menuItems = new List<MenuItem>();
        menuItems.Add(new MenuItem(){Name="Заказы"});
        if (employee.Role == "Администратор")
        {
            menuItems.Add(new MenuItem(){Name="Персонал"});
            menuItems.Add(new MenuItem(){Name="Статистика"});
            IsAdmin = true;
        }
        Menu = menuItems;
        SelectedMenuItem = Menu[0];
        return employee;
    }

    public List<MenuItem> Menu
    {
        get => _menu;
        set => this.RaiseAndSetIfChanged(ref _menu, value);
    }

    public MenuItem SelectedMenuItem
    {
        get => _selectedMenuItem;
        set => this.RaiseAndSetIfChanged(ref _selectedMenuItem, value);
    }

    public ViewModelBase SelectedViewModel
    {
        get => _selectedViewModel;
        set => this.RaiseAndSetIfChanged(ref _selectedViewModel, value);
    }
}