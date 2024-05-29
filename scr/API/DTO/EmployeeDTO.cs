using api.Entities;

namespace api.DTO;

public class EmployeeDTO
{
    public int Id { get; set; }
    public string Role { get; set; }
    public string Surname { get; set; }
    public string Name { get; set; }
    public string? Patronymic { get; set; }
    public string? ImagePath { get; set; }
    public EmployeeDTO(Employee employee)
    {
        Id = employee.Id;
        Role = employee.Role.Name;
        Surname = employee.Surname;
        Name = employee.Name;
        Patronymic = employee.Patronymic;
        ImagePath = employee.ImagePath;
    }
}