using System.Text.Json.Serialization;
using api.Entities;

namespace api.DTO;

public class EmployeeEditDTO
{
    [JsonConstructor]
    public EmployeeEditDTO() { }

    public EmployeeEditDTO(Employee employee)
    {
        Id = employee.Id;
        RoleId = employee.RoleId;
        Surname = employee.Surname;
        Name = employee.Name;
        Patronymic = employee.Patronymic;
        PhoneNumber = employee.PhoneNumber;
        Email = employee.Email;
        Login = employee.Login;
        Password = employee.Password;
        ImagePath = employee.ImagePath;
    }

    public int Id { get; set; }
    public int RoleId { get; set; }
    public string Surname { get; set; }
    public string Name { get; set; }
    public string? Patronymic { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? ImagePath { get; set; }
}