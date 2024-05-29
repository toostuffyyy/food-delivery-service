using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using desktop.Models;
using desktop.Service;
using desktop.Tools;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace desktop.ViewModels;

public class CatalogEmployeesViewModel : ViewModelBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IAccessTokenRepository _accessTokenRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ObservableAsPropertyHelper<bool> _isLoadingEmployees;
    private readonly IDialogService _dialogService;
    private readonly IViewNavigation _viewNavigation;
    private readonly ObservableAsPropertyHelper<IEnumerable<Employee>> _employees;
    // Gets Employee.
    private Employee _selectedEmployee;
    private ReactiveCommand<Unit, Unit> _cancelCommand;
    public IEnumerable<Employee> Employees => _employees.Value;
    public bool IsLoadingEmployees => _isLoadingEmployees.Value;
    public ReactiveCommand<Unit, EmployeeCollection> LoadingEmployeesCommand { get; private set; }
    // Pagination.
    private readonly ObservableAsPropertyHelper<int> _countPage;
    private readonly ObservableAsPropertyHelper<int> _countEmployees;
    public int CountPage => _countPage.Value;
    public int CountEmployees => _countEmployees.Value;
    public IEnumerable<int> PageCounts => new[] { 10, 15, 20, 30 };
    public OwnerParametersEmployee OwnerParameters { get; set; }
    // Filter.
    public List<FilterCategory> Filters { get; set; }
    // Sorting.
    public ObservableCollection<SortElement> SortElements { get; private set; }
    // Delete Employee.
    public ReactiveCommand<int, Unit> DeleteEmployeeCommand { set; get; }
    public Employee SelectedEmployee
    {
        get => _selectedEmployee;
        set => this.RaiseAndSetIfChanged(ref _selectedEmployee, value);
    }
    // Edit Employee.
    public ReactiveCommand<int, Unit> EditEmployeeCommand { get; private set; }
    private void EditEmployee(int id)
    {
        var bundle = new Bundle(this);
        bundle.AddNewParameter("idEmployee", id);
        _viewNavigation.GoTo<AddEditViewModel>(bundle);
    }
    
    public CatalogEmployeesViewModel(IEmployeeRepository employeeRepository,
                                IAccessTokenRepository accessTokenRepository,
                                INotificationService notificationService,
                                IUpdateTokenService updateTokenService,
                                IDialogService dialogService,
                                IRoleRepository roleRepository,
                                IViewNavigation viewNavigation) : base(notificationService, updateTokenService)
    {
        _employeeRepository = employeeRepository;
        _accessTokenRepository = accessTokenRepository;
        _roleRepository = roleRepository;
        _dialogService = dialogService;
        _viewNavigation = viewNavigation;
        
        // GetOrders.
        LoadingEmployeesCommand = ReactiveCommand
            .CreateFromObservable(() => Observable
                .StartAsync(ct => this.LoadingEmployeesTask(ct))
                .TakeUntil(_cancelCommand));
        LoadingEmployeesCommand.IsExecuting.ToProperty(this, a => a.IsLoadingEmployees, out _isLoadingEmployees);
        LoadingEmployeesCommand.ThrownExceptions.Subscribe(async exp => await CommandExc(exp, LoadingEmployeesCommand));
        _employees = LoadingEmployeesCommand.Where(a => a != null).Select(x => x.Employees)
            .ToProperty(this, a => a.Employees, scheduler: RxApp.MainThreadScheduler);
        _cancelCommand = ReactiveCommand.Create(() => { }, LoadingEmployeesCommand.IsExecuting);
        // Pagination.
        OwnerParameters = new OwnerParametersEmployee();
        _countEmployees = LoadingEmployeesCommand.Where(x => x != null).Select(a => a.Count)
            .ToProperty(this, x => x.CountEmployees, scheduler: RxApp.MainThreadScheduler);
        this.WhenAnyValue(a => a.CountEmployees, a => a.OwnerParameters.SizePage)
            .Where(x => x.Item2 != 0)
            .Select(x => (x.Item1 + x.Item2 - 1) / x.Item2)
            .ToProperty(this, x => x.CountPage, out _countPage);
        OwnerParameters.WhenAnyValue(a => a.PageNumber).Subscribe(_ => RestartLoadEmployees());
        OwnerParameters.WhenAnyValue(a => a.SizePage).Subscribe(_ => GoFirstAndRestartLoadEmployees());
        // Search.
        OwnerParameters.WhenAnyValue(a => a.SearchString).Where(a => !string.IsNullOrEmpty(a))
            .Throttle(TimeSpan.FromSeconds(0.5)).Subscribe(_ => GoFirstAndRestartLoadEmployees());
        // Filter.
        Filters = new List<FilterCategory>()
        {
            new() { NameCategory = "Роль", ParameterName = "role" }
        };
        OwnerParameters.WhenAnyValue(a => a.Filters).Subscribe(_ => GoFirstAndRestartLoadEmployees());
        // Sorting.
        SortElements = new ObservableCollection<SortElement>()
        {
            new() { Sorting = new() { NameColumn = "surname" }, NameSort = "Фамилия" },
            new() { Sorting = new() { NameColumn = "name" }, NameSort = "Имя" },
            new() { Sorting = new() { NameColumn = "role" }, NameSort = "Должность" }
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
        OwnerParameters.WhenAnyValue(a => a.Sorts).Subscribe(_ => GoFirstAndRestartLoadEmployees());
        ReactiveCommand.CreateFromTask(LoadFilters).Execute();
        // Delete Employee.
        DeleteEmployeeCommand = ReactiveCommand.CreateFromTask<int>(DeleteOrder);
        DeleteEmployeeCommand.ThrownExceptions.Subscribe(async exp => await CommandExc(exp, LoadingEmployeesCommand));
        // Edit Employee.
        EditEmployeeCommand = ReactiveCommand.Create<int>(EditEmployee);
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
        var employeeRoles = await _roleRepository.GetRole();
        Filters[0].Filters =
            new ObservableCollection<Filter>(employeeRoles.Select(a => new Filter()
                { NameFilter = a.Name, Value = a.Id }));
        
        Filters.ForEach(x => x.Filters.ToObservableChangeSet()
            .AutoRefresh(r => r.IsPick)
            .Subscribe(_ =>
            {
                var filterParameters = GetFilterParameters();
                OwnerParameters.Filters = JsonSerializer.Serialize(filterParameters);
            }));
    }
    
    private async Task<EmployeeCollection> LoadingEmployeesTask(CancellationToken ct)
    {
        var accessToken = _accessTokenRepository.GetAccessToken();
        if (ct.IsCancellationRequested)
            return null;
        var employeeCollection = await _employeeRepository.GetEmployees(accessToken, OwnerParameters);
        if (ct.IsCancellationRequested)
            return null;
        SelectedEmployee = employeeCollection.Employees.Count() > 0 ? employeeCollection.Employees.ToList()[0] : null;
        return employeeCollection;
    }
    // Pagination.
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
    
    private async Task DeleteOrder(int id)
    {
        var result = await _dialogService.ShowDialog("Удаление пользователя", "Вы действительно хотите удалить пользователя?",
            IDialogService.DialogType.YesNoDialog);
        if (result == IDialogService.DialogResult.Yes)
        {
            await _employeeRepository.DeleteEmployee(_accessTokenRepository.GetAccessToken(), id);
            GoFirstAndRestartLoadEmployees();
        }
    }
    
    public void AddEmployee()
    {
        Bundle bundle = new Bundle(this);
        _viewNavigation.GoTo<AddEditViewModel>(bundle);
    }
    
    private void RestartLoadEmployees()
    {
        _cancelCommand.Execute().Subscribe();
        LoadingEmployeesCommand.Execute().Subscribe();
    }

    public void GoFirstAndRestartLoadEmployees()
    {
        if (OwnerParameters.PageNumber == 1)
            RestartLoadEmployees();
        else OwnerParameters.PageNumber = 1;
    }
    public void GoFirstPage() => OwnerParameters.PageNumber = 1;
    public void GoLastPage() => OwnerParameters.PageNumber = CountPage;
}