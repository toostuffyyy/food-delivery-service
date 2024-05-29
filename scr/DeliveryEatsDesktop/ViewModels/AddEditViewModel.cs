using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using desktop.Models;
using desktop.Service;
using desktop.Services;
using ReactiveUI;
using Refit;
using Splat;
using Notification = Avalonia.Controls.Notifications.Notification;

namespace desktop.ViewModels;

public class AddEditViewModel : ViewModelBase
{
    private readonly IViewNavigation _viewNavigation;
    private readonly IAccessTokenRepository _accessTokenRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IFilePickerService _filePickerService;
    private readonly IImageService _imageService;

    private EmployeeEdit _employeeEdit;
    public EmployeeEdit EmployeeEdit
    {
        get => _employeeEdit;
        set => this.RaiseAndSetIfChanged(ref _employeeEdit, value);
    }
    private readonly Lazy<ReactiveCommand<int, Unit>> _lazyGetEmployeeEdit;
    private readonly ObservableAsPropertyHelper<IEnumerable<EmployeeRole>> _role;

    public Authorization Authorization { get; set; }
    public IEnumerable<EmployeeRole> EmployeeRole => _role.Value;
    public ReactiveCommand<Unit, IEnumerable<EmployeeRole>> GetRoleCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; private set; }
    public ReactiveCommand<string, Unit> ChangeImageProductCommand { get; }
    public ReactiveCommand<string, Unit> RemoveImageCommand { get; }

    public AddEditViewModel(IViewNavigation viewNavigation, 
            IAccessTokenRepository accessTokenRepository,
            IEmployeeRepository employeeRepository,
            IRoleRepository roleRepository,
            IFilePickerService filePickerService, 
            IImageService imageService,
            IUpdateTokenService updateTokenService, 
            INotificationService notificationService,
            IAuthorizationRepository authorizationRepository) : base(notificationService, updateTokenService)
    {
        _viewNavigation = viewNavigation;
        _accessTokenRepository = accessTokenRepository;
        _employeeRepository = employeeRepository;
        _roleRepository = roleRepository;
        _filePickerService = filePickerService;
        _imageService = imageService;

        Authorization = new Authorization();
        
        _lazyGetEmployeeEdit = new Lazy<ReactiveCommand<int, Unit>>(() =>
        {
            var command = ReactiveCommand.CreateFromTask<int>(GetEmployee);
            command.ThrownExceptions.Subscribe(async x => await CommandExc(x, command));
            return command;
        });
        
        GetRoleCommand = ReactiveCommand.CreateFromTask(async () => await _roleRepository.GetRole());
        GetRoleCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, GetRoleCommand));
        _role = GetRoleCommand.ToProperty(this, x => x.EmployeeRole);
        GetRoleCommand.Subscribe(_ =>
        {
            object? index = Bundle.GetParameter("idEmployee");
            if (index != null)
                _lazyGetEmployeeEdit.Value.Execute((int)index).Subscribe();
            else 
                EmployeeEdit = new EmployeeEdit();
        });
        
        SaveCommand = ReactiveCommand.CreateFromTask(Save);
        SaveCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, SaveCommand));

        this.WhenAnyValue(x => x.Bundle).Where(x => x != null).Subscribe(_ =>
        {
            if (Bundle.GetParameter("idEmployee") != null)
                Title = "Редактирование данных сотрудника";
            else
                Title = "Добавление данных сотрудника";
            GetRoleCommand.Execute().Subscribe();
        });

        ChangeImageProductCommand = ReactiveCommand.CreateFromTask<string>(ChangeImageProduct);
        ChangeImageProductCommand.ThrownExceptions.Subscribe(async x => await CommandExc(x, ChangeImageProductCommand));
        RemoveImageCommand = ReactiveCommand.Create<string>((par) =>
        {
            EmployeeEdit.ImagePath = null;
        });
    }

    private async Task Save()
    {
        if (!string.IsNullOrEmpty(Authorization.Login) && !string.IsNullOrEmpty(Authorization.Password))
            (EmployeeEdit.Login, EmployeeEdit.Password) = (Authorization.Login, Authorization.Password);
        
        if (Bundle.GetParameter("idEmployee") != null)
            await _employeeRepository.UpdateEmployee(_accessTokenRepository.GetAccessToken(), EmployeeEdit);
        else 
            await _employeeRepository.AddEmployee(_accessTokenRepository.GetAccessToken(), EmployeeEdit);
        
        (Bundle?.OwnerViewModel as CatalogEmployeesViewModel)?.GoFirstAndRestartLoadEmployees();
        _viewNavigation.Close(this);
    }
    
    private async Task ChangeImageProduct(string parameter)
    {
        await using var stream = await _filePickerService.OpenFile(IFilePickerService.Filter.JpgImage);
        if (stream == null)
            return;
        StreamPart file = new StreamPart(stream, "image.jpg", contentType:"image/jpeg");
        string fileName = await _imageService.UploadImage(_accessTokenRepository.GetAccessToken(), file, "Employee");
        EmployeeEdit.ImagePath = fileName;
    }
    
    private async Task GetEmployee(int id)
    {
        var employee = await _employeeRepository.GetEmployeeEdit(_accessTokenRepository.GetAccessToken(), id);
        EmployeeEdit = employee;
    }
}