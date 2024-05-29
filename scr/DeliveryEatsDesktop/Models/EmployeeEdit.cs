using ReactiveUI;

namespace desktop.Models;

public class EmployeeEdit : ReactiveObject
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string Surname { get; set; }
    public string Name { get; set; }
    public string? Patronymic { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
    private string? _image;
    public string? ImagePath
    {
        get => _image;
        set => this.RaiseAndSetIfChanged(ref _image, value);
    }
}